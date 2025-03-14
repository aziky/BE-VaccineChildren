using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Application.DTOs.Response;
using VaccineChildren.Application.Services;
using VaccineChildren.Core.Base;

namespace VaccineChildren.API.Controllers;

[Route("api/[controller]")]
[ApiController]
// [Authorize(Roles = "user")]
public class UserController : BaseController
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;

    public UserController(ILogger<UserController> logger, IUserService userService, IConfiguration configuration)
    {
        _logger = logger;
        _userService = userService;
        _configuration = configuration;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserReq userReq)
    {
        try
        {
            var userRes = await _userService.Login(userReq);
            return Ok(BaseResponse<UserRes>.OkResponse(userRes, "Login successful"));
        }
        catch (Exception ex)
        {
            _logger.LogError("{Classname} - Error at get account async cause by {}", nameof(UserController), ex.Message);
            return HandleException(ex, nameof(UserController));
        }
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
    {
        try
        {
            // await _userService.RegisterUserAsync(registerRequest);
            RegisterResponse registerRes = await _userService.RegisterUserAsync(registerRequest);
            if (registerRes.Success == false)
            {
                return BadRequest(BaseResponse<RegisterResponse>.BadRequestResponse(mess: registerRes.Message));
            }
            return Ok(BaseResponse<string>.OkResponse(mess: "User registered successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError("{Classname} - Error at get account async cause by {}", nameof(UserController), ex.Message);
            return HandleException(ex, nameof(UserController));
        }
    }

    [AllowAnonymous]
    [HttpPost("google-login")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleAuthRequest request)
    {
        try
        {
            var userRes = await _userService.LoginWithGoogleAsync(request);
            return Ok(BaseResponse<UserRes>.OkResponse(userRes, "Google login successful"));
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Unauthorized Google login attempt: {Message}", ex.Message);
            return Unauthorized(BaseResponse<string>.BadRequestResponse("Invalid Google credentials"));
        }
        catch (Exception ex)
        {
            _logger.LogError("{Classname} - Error during Google login: {Message}", nameof(UserController), ex.Message);
            return HandleException(ex, nameof(UserController));
        }
    }

    [AllowAnonymous]
    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string token, [FromQuery] string email)
    {
        try
        {
            var result = await _userService.VerifyEmailAsync(token, email);

            if (!result.Success)
            {
                return BadRequest(BaseResponse<string>.BadRequestResponse("Email verification failed"));
            }

            // Redirect to frontend success page
            return Redirect(
                $"{_configuration["FrontendUrl"]}/login");
        }
        catch (Exception ex)
        {
            _logger.LogError("{Classname} - Error verifying email cause by {}", nameof(UserController), ex.Message);
            return HandleException(ex, nameof(UserController));
        }
    }

    [AllowAnonymous]
    [HttpGet("resend-verification")]
    public async Task<IActionResult> ResendVerificationEmail([FromQuery] string email)
    {
        try
        {
            var result = await _userService.ResendVerificationEmailAsync(email);

            if (!result.Success)
            {
                return BadRequest(BaseResponse<string>.BadRequestResponse("Resend email verification failed"));
            }

            return Ok(BaseResponse<string>.OkResponse("Verification email sent successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError("{Classname} - Error resending verification email cause by {}", nameof(UserController),
                ex.Message);
            return HandleException(ex, nameof(UserController));
        }
    }

    [HttpPost("child")]
    public async Task<IActionResult> CreateChildAsync([FromBody] CreateChildReq request)
    {
        try
        {
            await _userService.CreateChildAsync(request);
            return Ok(BaseResponse<string>.CreateResponse("Child created successfully"));
        }
        catch (Exception e)
        {
            _logger.LogError("Error at create child async cause by {}", e.Message);
            return HandleException(e, nameof(UserController));
        }
    }

    [HttpGet("child/{childId}")]
    public async Task<IActionResult> GetChildAsync([FromRoute] string childId)
    {
        try
        {
            var childRes = await _userService.GetChildByChildIdAsync(childId);
            return Ok(BaseResponse<GetChildRes>.OkResponse(childRes, "child profile"));
        }
        catch (Exception e)
        {
            _logger.LogError("Error at get child async cause by {}", e.Message);
            return HandleException(e, nameof(UserController));
        }
    }


    [HttpGet("{userId}")]
    public async Task<IActionResult> GetListChildByUserIdAsync([FromRoute] string userId)
    {
        try
        {
            var userProfile = await _userService.GetUserByUserIdAsync(userId);
            return Ok(BaseResponse<GetUserRes>.OkResponse(userProfile, "get user profile successfully"));
        }
        catch (Exception e)
        {
            _logger.LogError("Error at get child async cause by {}", e.Message);
            return HandleException(e, nameof(UserController));
        }
    }

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        try
        {
            var result = await _userService.ForgotPasswordAsync(request.Email);

            if (!result.Success)
            {
                return BadRequest(BaseResponse<string>.BadRequestResponse(result.Message));
            }

            return Ok(BaseResponse<string>.OkResponse("Password reset email sent successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError("{Classname} - Error sending password reset email: {}", nameof(UserController),
                ex.Message);
            return HandleException(ex, nameof(UserController));
        }
    }

    [AllowAnonymous]
    [HttpGet("verify-reset-token")]
    public async Task<IActionResult> VerifyResetToken([FromQuery] string token, [FromQuery] string email)
    {
        try
        {
            var result = await _userService.VerifyResetTokenAsync(token, email);

            if (!result.Success)
            {
                return BadRequest(BaseResponse<string>.BadRequestResponse("Invalid or expired reset token"));
            }

            // Redirect to frontend reset password page with token and email parameters
            return Redirect(
                $"{_configuration["FrontendUrl"]}/reset-password?token={token}&email={Uri.EscapeDataString(email)}");
        }
        catch (Exception ex)
        {
            _logger.LogError("{Classname} - Error verifying reset token: {}", nameof(UserController), ex.Message);
            return HandleException(ex, nameof(UserController));
        }
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        try
        {
            var result = await _userService.ResetPasswordAsync(request);

            if (!result.Success)
            {
                return BadRequest(BaseResponse<string>.BadRequestResponse(result.Message));
            }

            return Ok(BaseResponse<string>.OkResponse("Password reset successful"));
        }
        catch (Exception ex)
        {
            _logger.LogError("{Classname} - Error resetting password: {}", nameof(UserController), ex.Message);
            return HandleException(ex, nameof(UserController));
        }
    }
}