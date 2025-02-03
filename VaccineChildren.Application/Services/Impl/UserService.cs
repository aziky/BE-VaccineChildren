using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Application.DTOs.Response;
using VaccineChildren.Domain.Abstraction;
using VaccineChildren.Domain.Entities;

namespace VaccineChildren.Application.Services.Impl;

public class UserService : IUserService
{
    private readonly ILogger<IUserService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly string _jwtSecret;
    private readonly RsaService _rsaService;

    public UserService(ILogger<IUserService> logger, IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration, RsaService rsaService)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _jwtSecret = configuration["Jwt:Secret"];
        _rsaService = rsaService;
    }
    
    // public async Task<RegisterResponse> RegisterUserAsync(RegisterRequest registerRequest)
    // {
    //     try
    //     {
    //         _logger.LogInformation("Start registering user");
    //         _unitOfWork.BeginTransaction();
    //
    //         var userRepository = _unitOfWork.GetRepository<User>();
    //
    //         // Hash password using RSA
    //         var hashedPassword = HashPassword(registerRequest.Password);
    //         registerRequest.Password = hashedPassword;
    //
    //         var userEntity = _mapper.Map<User>(registerRequest);
    //         await userRepository.InsertAsync(userEntity);
    //         await _unitOfWork.SaveChangeAsync();
    //
    //         _unitOfWork.CommitTransaction();
    //         _logger.LogInformation("User registration successful");
    //
    //         return new RegisterResponse
    //         {
    //             Success = true,
    //             Message = "User registered successfully"
    //         };
    //     }
    //     catch (Exception e)
    //     {
    //         _logger.LogError("Error at register user: {Message}", e.Message);
    //         _unitOfWork.RollBack();
    //         throw;
    //     }
    //     finally
    //     {
    //         _unitOfWork.Dispose();
    //     }
    // }
    public async Task<RegisterResponse> RegisterUserAsync(RegisterRequest registerRequest)
    {
        try
        {
            _logger.LogInformation("Start registering user");
            _unitOfWork.BeginTransaction();

            var userRepository = _unitOfWork.GetRepository<User>();

            // Check if email or username already exists
            var existingUser = await userRepository.FindByConditionAsync(
                u => u.Email == registerRequest.Email || u.UserName == registerRequest.UserName);

            if (existingUser != null)
            {
                _logger.LogWarning("Email or Username already exists: {Email} or {Username}", 
                    registerRequest.Email, registerRequest.UserName);

                return new RegisterResponse
                {
                    Success = false,
                    Message = "Email or Username already exists"
                };
            }
            // var hashedPassword = HashPassword(registerRequest.Password);
            // registerRequest.Password = hashedPassword;

            // Map request to User entity
            var userEntity = _mapper.Map<User>(registerRequest);
            userEntity.CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            // Insert user into database
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

            // Find user by username
            var user = await userRepository.FindByConditionAsync(u => u.UserName == userReq.UserName);
            if (user == null || !VerifyPassword(userReq.Password, user.Password))
            {
                _logger.LogInformation("Login failed");
                throw new KeyNotFoundException("Invalid username or password");
            }

            // Generate JWT token
            var token = GenerateJwtToken(user);
            var response = _mapper.Map<UserRes>(user);
            response.Token = token;

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

    // private string HashPassword(string password)
    // {
    //     using var rsa = RSA.Create();
    //     var passwordBytes = Encoding.UTF8.GetBytes(password);
    //     var encryptedBytes = rsa.Encrypt(passwordBytes, RSAEncryptionPadding.Pkcs1);
    //     return Convert.ToBase64String(encryptedBytes);
    // }

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
