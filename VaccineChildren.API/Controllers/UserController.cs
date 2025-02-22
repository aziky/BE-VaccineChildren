using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Application.DTOs.Response;
using VaccineChildren.Application.Services;
using VaccineChildren.Core.Base;

namespace VaccineChildren.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize (Roles = "user")]
public class UserController : BaseController
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;

    public UserController(ILogger<UserController> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
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
            _logger.LogError("Error at create child async cause by {}",  e.Message);
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
            _logger.LogError("Error at get child async cause by {}",  e.Message);
            return HandleException(e, nameof(UserController));
        }
    }
    
    
    [HttpGet]
    public async Task<IActionResult> GetListChildByUserIdAsync()
    {
        try
        {
            var userProfile = await _userService.GetUserByUserIdAsync();
            return Ok(BaseResponse<GetUserRes>.OkResponse(userProfile, "get user profile successfully"));
        }
        catch (Exception e)
        {
            _logger.LogError("Error at get child async cause by {}",  e.Message);
            return HandleException(e, nameof(UserController));
        }
    }
    
    
}