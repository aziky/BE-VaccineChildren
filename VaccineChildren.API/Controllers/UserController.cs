using Microsoft.AspNetCore.Mvc;
using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Application.DTOs.Response;
using VaccineChildren.Application.Services;
using VaccineChildren.Core.Base;

namespace VaccineChildren.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : BaseController
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;

    public UserController(ILogger<UserController> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

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
    
}