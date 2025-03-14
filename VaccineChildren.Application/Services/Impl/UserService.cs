using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Application.DTOs.Response;
using VaccineChildren.Core.Store;
using VaccineChildren.Domain.Abstraction;
using VaccineChildren.Domain.Entities;

namespace VaccineChildren.Application.Services.Impl;

public class UserService : IUserService
{
    private const string DateFormat = "dd-MM-yyyy";
    
    private readonly ILogger<IUserService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IRsaService _rsaService;
    private readonly ICacheService _cacheService;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IGoogleAuthService _googleAuthService;

    public UserService(ILogger<IUserService> logger, IUnitOfWork unitOfWork, IMapper mapper, 
        IConfiguration configuration, IRsaService rsaService, ICacheService cacheService,
        IEmailService emailService, IHttpContextAccessor httpContextAccessor,IGoogleAuthService googleAuthService)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _rsaService = rsaService;
        _cacheService = cacheService;
        _emailService = emailService;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
        _googleAuthService = googleAuthService;
    }
public async Task<RegisterResponse> RegisterUserAsync(RegisterRequest registerRequest)
{
    try
    {
        _logger.LogInformation("Start registering user");

        var userRepository = _unitOfWork.GetRepository<User>();

        // Check if email already exists
        var existingUser = await userRepository.FindByConditionAsync(u => u.Email == registerRequest.Email);

        if (existingUser != null)
        {
            _logger.LogWarning("Email already exists: {Email}", registerRequest.Email);
            return new RegisterResponse
            {
                Success = false,
                Message = "Email already exists"
            };
        }

        // Hash the password
        var hashedPassword = _rsaService.Encrypt(registerRequest.Password);
        registerRequest.Password = hashedPassword;

        // Map request to User entity (but don't save to DB yet)
        var userEntity = _mapper.Map<User>(registerRequest);
        userEntity.CreatedBy = registerRequest.UserName;
        userEntity.CreatedAt = DateTime.UtcNow.ToLocalTime();
        userEntity.IsVerified = false;
        
        // Get the user role
        var roleRepository = _unitOfWork.GetRepository<Role>();
        var userRole = await roleRepository.FindByConditionAsync(
            r => r.RoleName.ToLower() == StaticEnum.RoleEnum.User.ToString().ToLower());
        
        if (userRole == null)
        {
            _logger.LogError("User role not found in database");
            return new RegisterResponse
            {
                Success = false,
                Message = "Error assigning user role"
            };
        }

        userEntity.RoleId = userRole.RoleId;
        
        // Generate verification token
        var verificationToken = Guid.NewGuid().ToString();
        var expiration = TimeSpan.FromDays(1);
        
        // Store user information and verification token in Redis
        var userRegistrationData = new UserRegistrationData
        {
            VerificationToken = verificationToken,
            UserData = userEntity
        };
        
        // Store in Redis with expiration
        await _cacheService.SetAsync($"pending_registration:{registerRequest.Email}", 
            userRegistrationData, expiration);
        
        // Send verification email
        await SendVerificationEmail(userEntity, verificationToken);

        _logger.LogInformation("User registration data stored in Redis, verification email sent");

        return new RegisterResponse
        {
            Success = true,
            Message = "Registration successful. Please check your email to verify your account."
        };
    }
    catch (Exception e)
    {
        _logger.LogError("Error at register user: {Message}", e.Message);
        throw new Exception("An error occurred while registering the user", e);
    }
}
    public async Task<UserRes> LoginWithGoogleAsync(GoogleAuthRequest request)
{
    try
    {
        _logger.LogInformation("Processing Google login");
        
        // Xác thực token Google
        var googleUserInfo = await _googleAuthService.VerifyGoogleTokenAsync(request.IdToken);
        
        if (googleUserInfo == null || string.IsNullOrEmpty(googleUserInfo.Email))
        {
            throw new UnauthorizedAccessException("Invalid Google token");
        }
        
        // Kiểm tra xem email đã tồn tại trong hệ thống chưa
        var userRepository = _unitOfWork.GetRepository<User>();
        var user = await userRepository.FindByConditionAsync(u => u.Email == googleUserInfo.Email);
        
        // Nếu email chưa có trong hệ thống, đăng ký người dùng mới
        if (user == null)
        {
            string randomPassword = Guid.NewGuid().ToString() + Guid.NewGuid().ToString();
            string hashedPassword = _rsaService.Encrypt(randomPassword);
            user = new User
            {
                Email = googleUserInfo.Email,
                UserName = request.Username,
                Password = hashedPassword,
                FullName = request.Username,
                IsVerified = googleUserInfo.VerifiedEmail,
                CreatedAt = DateTime.UtcNow.ToLocalTime(),
                CreatedBy = "Google Auth"
            };
            
            // Gán role "user"
            var roleRepository = _unitOfWork.GetRepository<Role>();
            var userRole = await roleRepository.FindByConditionAsync(
                r => r.RoleName.ToLower() == StaticEnum.RoleEnum.User.ToString().ToLower());
            
            if (userRole == null)
            {
                _logger.LogError("User role not found in database");
                throw new InvalidOperationException("Error assigning user role");
            }
            
            user.RoleId = userRole.RoleId;
            
            await userRepository.InsertAsync(user);
            await _unitOfWork.SaveChangeAsync();
        }
        
        // Tạo JWT token
        var token = GenerateJwtToken(user);
        
        // Tạo response
        var response = _mapper.Map<UserRes>(user);
        response.Token = token;
        response.RoleName = user.Role?.RoleName;
        
        // Cache thông tin người dùng
        string cacheKey = $"user_{user.Email}";
        await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromHours(1));
        
        return response;
    }
    catch (Exception e)
    {
        _logger.LogError("Error during Google login: {Message}", e.Message);
        throw;
    }
}

    private async Task SendVerificationEmail(User user, string verificationToken)
    {
        // Create verification URL with token
        string verificationUrl = $"{_configuration["AppUrl"]}/api/User/verify-email?token={verificationToken}&email={Uri.EscapeDataString(user.Email)}";
    
        // Get the email template
        var templateId = StaticEnum.EmailTemplateEnum.EmailVerification.Id();
    
        var templateData = new Dictionary<string, string>
        {
            { "UserName", user.UserName },
            { "VerificationLink", verificationUrl },
            { "VerificationButton", $"<a href='{verificationUrl}' style='display:inline-block;background-color:#c10c0c;color:white;padding:10px 20px;text-decoration:none;border-radius:5px;font-weight:bold;'>CONFIRM YOUR EMAIL</a>" }
        };
    
        // Send the email
        await _emailService.SendEmailAsync(
            user.Email,
            user.UserName,
            templateId,
            templateData
        );
    }
    
    public async Task<RegisterResponse> VerifyEmailAsync(string token, string email)
    {
        try
        {
            // Get the pending registration data from Redis
            var registrationData = await _cacheService.GetAsync<UserRegistrationData>($"pending_registration:{email}");
        
            if (registrationData == null || registrationData.VerificationToken != token)
            {
                return new RegisterResponse { Success = false, Message = "Invalid or expired verification link" };
            }
        
            // Begin transaction to save the user to the database
            _unitOfWork.BeginTransaction();
        
            try
            {
                // Get the User entity from Redis
                var user = registrationData.UserData;
            
                // Set verification status to true
                user.IsVerified = true;
            
                // Save user to database
                var userRepository = _unitOfWork.GetRepository<User>();
                await userRepository.InsertAsync(user);
                await _unitOfWork.SaveChangeAsync();
            
                // Remove the pending registration data from Redis
                await _cacheService.RemoveAsync($"pending_registration:{email}");
            
                _unitOfWork.CommitTransaction();
            
                return new RegisterResponse { Success = true, Message = "Email verified successfully and account activated" };
            }
            catch (Exception ex)
            {
                _unitOfWork.RollBack();
                _logger.LogError(ex, "Error saving verified user to database for {Email}", email);
                return new RegisterResponse { Success = false, Message = "Error activating account" };
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying email for {Email}", email);
            throw;
        }
    }
    public async Task<RegisterResponse> ResendVerificationEmailAsync(string email)
    {
        try
        {
            var userRepository = _unitOfWork.GetRepository<User>();
            var user = await userRepository.FindByConditionAsync(u => u.Email == email);

            if (user == null)
            {
                return new RegisterResponse { Success = false, Message = "User not found" };
            }

            if (user.IsVerified)
            {
                return new RegisterResponse { Success = false, Message = "Email is already verified" };
            }

            // Generate a new verification token
            string verificationToken = Guid.NewGuid().ToString();

            // Store token in Redis with expiration (1 day)
            await _cacheService.SetAsync(
                $"email_verification:{email}",
                new EmailVerificationData { Token = verificationToken },
                TimeSpan.FromDays(1)
            );

            // Send the verification email with the new token
            await SendVerificationEmail(user, verificationToken);

            return new RegisterResponse { Success = true, Message = "Verification email has been resent" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resending verification email for {Email}", email);
            throw;
        }
    }
    public async Task<UserRes> Login(UserReq userReq)
    {
        try
        {
            var userRepository = _unitOfWork.GetRepository<User>();
            var user = await userRepository.FindByConditionAsync(u => u.Email == userReq.Email);
            
            if (user == null || !VerifyPassword(userReq.Password, user.Password))
            {
                _logger.LogInformation("Login failed");
                throw new KeyNotFoundException("Invalid username or password");
            }

            // Chỉ get cache sau khi đã xác thực password thành công
            string cacheKey = $"user_{userReq.Email}";
            var cachedUser = await _cacheService.GetAsync<UserRes>(cacheKey);
            if (cachedUser != null)
            {
                return cachedUser;
            }

            var token = GenerateJwtToken(user);
            var response = _mapper.Map<UserRes>(user);
            response.Token = token;
            response.RoleName = user.Role?.RoleName ?? "Unknown";

            await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromHours(1));

            return response;
        }
        catch (Exception e)
        {
            _logger.LogError("Error at login: {Message}", e.Message);
            throw;
        }
        finally
        {
            _unitOfWork.Dispose();
        }
    }

    

    private bool VerifyPassword(string inputPassword, string encryptedPassword)
    {
        string decryptedPassword = _rsaService.Decrypt(encryptedPassword);
        return decryptedPassword != null && inputPassword == decryptedPassword;
    }

    private string GenerateJwtToken(User user)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var signingCredentials = _rsaService.GetSigningCredentials();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.UserId.ToString()),
                    new Claim(ClaimTypes.Role, user.Role?.RoleName ?? "User"),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = signingCredentials,
                Issuer = "VaccineChildren",
                Audience = "VaccineChildren.API"
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error generating JWT token: {Message}", ex.Message);
            throw new InvalidOperationException("Failed to generate JWT token.", ex);
        }
    }
    
     public async Task CreateChildAsync(CreateChildReq request)
    {
        try
        {
            _logger.LogInformation("Start creating child profile");
            _unitOfWork.BeginTransaction();
            var childRepository = _unitOfWork.GetRepository<Child>();
            var userRepository = _unitOfWork.GetRepository<User>();
            
            
            var user = await userRepository.GetByIdAsync(Guid.Parse(request.UserId));
            if (user == null)
            {
                _logger.LogError("User not found");
                throw new KeyNotFoundException("User not found");
            }

            _logger.LogInformation("Start creating child");
            var child = _mapper.Map<Child>(request);
            child.Dob = DateOnly.ParseExact(request.Dob, DateFormat, CultureInfo.InvariantCulture);
            child.CreatedAt = DateTime.Now;
            child.CreatedBy = user.Email;
            await childRepository.InsertAsync(child);
            await _unitOfWork.SaveChangeAsync();
            _unitOfWork.CommitTransaction();
            
            _logger.LogInformation("Child created successfully");
        }
        catch (Exception e)
        {
            _unitOfWork.RollBack();
            _logger.LogError("Error at create child async cause by: {Message}", e.Message);
            throw;
        }
        finally
        {
            _unitOfWork.Dispose();
        }
    }

    public async Task<GetChildRes> GetChildByChildIdAsync(string childId)
    {
        try
        {
            _logger.LogInformation("Start getting child profile");
            
            var childRepository = _unitOfWork.GetRepository<Child>();
            var child = await childRepository.GetAllAsync(query => query
                .Include(c => c.Schedules.Where(s => s.status == StaticEnum.ScheduleStatusEnum.Completed.Name()))
                .ThenInclude(s => s.Vaccine).ThenInclude(v => v.VaccineManufactures).ThenInclude(vm => vm.Manufacturer)
                .Where(c => c.ChildId.ToString().Equals(childId))
            );
            if (child.FirstOrDefault() == null) throw new KeyNotFoundException("Child not found");
            
            return _mapper.Map<GetChildRes>(child.FirstOrDefault());
        }
        catch (Exception e)
        {
            _logger.LogError("Error at get child async cause by: {}", e.Message);
            throw;
        }
    }

    public async Task<GetUserRes> GetUserByUserIdAsync(string userId)
    {
        try
        {
            _logger.LogInformation("Start get user profile with user id {}", userId);
            var userRepository = _unitOfWork.GetRepository<User>();

            var user = await userRepository.GetAllAsync(query => query.Include(u => u.Children)
                .ThenInclude(c => c.Schedules.Where(s => s.status == StaticEnum.ScheduleStatusEnum.Completed.Name()))
                .ThenInclude(s => s.Vaccine).ThenInclude(v => v.VaccineManufactures).ThenInclude(vm => vm.Manufacturer)
                    .Where(u => u.UserId.ToString().Equals(userId)));
            if (user.FirstOrDefault() == null) throw new KeyNotFoundException("Can't find user profile with user id: " + userId);
            
            return _mapper.Map<GetUserRes>(user.FirstOrDefault());
            
        }
        catch (Exception e)
        {
            _logger.LogError("Error at get user by user id async cause by: {}", e.Message);
            throw;
        }
    }
    public async Task<RegisterResponse> ForgotPasswordAsync(string email)
{
    try
    {
        _logger.LogInformation("Processing forgot password request for email: {Email}", email);
        
        var userRepository = _unitOfWork.GetRepository<User>();
        var user = await userRepository.FindByConditionAsync(u => u.Email == email);

        if (user == null)
        {
            // For security reasons, don't reveal that the user doesn't exist
            _logger.LogWarning("Forgot password requested for non-existent email: {Email}", email);
            return new RegisterResponse { Success = true, Message = "If your email exists in our system, you will receive a password reset link" };
        }

        // Generate a password reset token
        string resetToken = Guid.NewGuid().ToString();
        
        // Store token in Redis with expiration (1 hour)
        await _cacheService.SetAsync(
            $"password_reset:{email}",
            new PasswordResetData { 
                Token = resetToken,
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            },
            TimeSpan.FromHours(1)
        );

        // Send password reset email
        await SendPasswordResetEmail(user, resetToken);

        return new RegisterResponse { Success = true, Message = "Password reset email sent successfully" };
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error processing forgot password request for {Email}", email);
        throw;
    }
}

private async Task SendPasswordResetEmail(User user, string resetToken)
{
    // Create reset password URL with token
    string resetUrl = $"{_configuration["AppUrl"]}/api/User/verify-reset-token?token={resetToken}&email={Uri.EscapeDataString(user.Email)}";
    
    // Get the email template (assuming you have a template for password reset)
    var templateId = StaticEnum.EmailTemplateEnum.PasswordReset.Id();
    
    var templateData = new Dictionary<string, string>
    {
        { "UserName", user.UserName },
        { "ResetLink", resetUrl },
        { "ResetButton", $"<a href='{resetUrl}' style='display:inline-block;background-color:#c10c0c;color:white;padding:10px 20px;text-decoration:none;border-radius:5px;font-weight:bold;'>RESET YOUR PASSWORD</a>" }
    };
    
    // Send the email
    await _emailService.SendEmailAsync(
        user.Email,
        user.UserName,
        templateId,
        templateData
    );
}

public async Task<RegisterResponse> VerifyResetTokenAsync(string token, string email)
{
    try
    {
        _logger.LogInformation("Verifying password reset token for email: {Email}", email);
        
        // Get the reset token data from Redis
        var resetData = await _cacheService.GetAsync<PasswordResetData>($"password_reset:{email}");
        
        if (resetData == null || resetData.Token != token || resetData.ExpiresAt < DateTime.UtcNow)
        {
            return new RegisterResponse { Success = false, Message = "Invalid or expired reset token" };
        }
        
        return new RegisterResponse { Success = true, Message = "Token verified successfully" };
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error verifying reset token for {Email}", email);
        throw;
    }
}

public async Task<RegisterResponse> ResetPasswordAsync(ResetPasswordRequest request)
{
    try
    {
        _logger.LogInformation("Processing password reset for email: {Email}", request.Email);
        
        // First verify the token again
        var resetData = await _cacheService.GetAsync<PasswordResetData>($"password_reset:{request.Email}");
        
        if (resetData == null || resetData.Token != request.Token || resetData.ExpiresAt < DateTime.UtcNow)
        {
            return new RegisterResponse { Success = false, Message = "Invalid or expired reset token" };
        }
        
        // Update the user's password
        var userRepository = _unitOfWork.GetRepository<User>();
        var user = await userRepository.FindByConditionAsync(u => u.Email == request.Email);
        
        if (user == null)
        {
            return new RegisterResponse { Success = false, Message = "User not found" };
        }
        
        // Hash the new password
        string hashedPassword = _rsaService.Encrypt(request.NewPassword);
        user.Password = hashedPassword;
        user.UpdatedAt = DateTime.UtcNow.ToLocalTime();
        user.UpdatedBy = "System";
        
        await userRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangeAsync();
        
        // Remove the reset token from Redis
        await _cacheService.RemoveAsync($"password_reset:{request.Email}");
        
        // Invalidate any cached user data
        await _cacheService.RemoveAsync($"user_{request.Email}");
        
        return new RegisterResponse { Success = true, Message = "Password reset successful" };
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error resetting password for {Email}", request.Email);
        throw;
    }
}
}
