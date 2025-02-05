using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Application.DTOs.Response;
using VaccineChildren.Domain.Abstraction;
using VaccineChildren.Domain.Entities;
using VaccineChildren.Core.Store;
using VaccineChildren.Application.Services;

namespace VaccineChildren.Application.Services.Impl;

public class UserService : IUserService
{
    private readonly ILogger<IUserService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly string _jwtSecret;
    private readonly RsaService _rsaService;
    private readonly ICacheService _cacheService;

    public UserService(ILogger<IUserService> logger, IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration, RsaService rsaService, ICacheService cacheService)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _jwtSecret = configuration["Jwt:Secret"];
        _rsaService = rsaService;
        _cacheService = cacheService;
    }
    public async Task<RegisterResponse> RegisterUserAsync(RegisterRequest registerRequest)
    {
        try
        {
            _logger.LogInformation("Start registering user");
            _unitOfWork.BeginTransaction();

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

            var hashedPassword = _rsaService.Encrypt(registerRequest.Password);
            registerRequest.Password = hashedPassword;

            // Map request to User entity
            var userEntity = _mapper.Map<User>(registerRequest);
            userEntity.CreatedAt = DateTime.UtcNow.ToLocalTime();

            // Assign the 'user' role to the new user
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

            await userRepository.InsertAsync(userEntity);
            await _unitOfWork.SaveChangeAsync();

            _unitOfWork.CommitTransaction();
            _logger.LogInformation("User registration successful");

            return new RegisterResponse
            {
                Success = true,
                Message = "User registered successfully"
            };
        }
        catch (Exception e)
        {
            _logger.LogError("Error at register user: {Message}", e.Message);
            _unitOfWork.RollBack();
            throw new Exception("An error occurred while registering the user", e);
        }
        finally
        {
            _unitOfWork.Dispose();
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
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSecret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.UserId.ToString()),
                new Claim(ClaimTypes.Role, user.Role?.RoleName ?? "User")
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
