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

    [HttpPost("/login")]
    public async Task<IActionResult> Login([FromBody] UserReq userReq)
    {
        try
        {
            var userRes = await _userService.Login(userReq);
            return Ok(BaseResponse<UserRes>.OkResponse(userRes, "Login successful"));
        }
        catch (KeyNotFoundException e)
        {
            _logger.LogError("Error at login: {}", e.Message);
            return BadRequest("Invalid username or password");
        }
        catch (Exception e)
        {
            _logger.LogError("Error at login: {}", e.Message);
            return HandleException(e, "Internal Server Error");
        }
    }

    [HttpPost("/register")]
    public async Task<IActionResult> Register([FromBody] UserReq userReq)
    {
        try
        {
             await _userService.RegisterUser(userReq);
             return Ok(BaseResponse<string>.OkResponse(mess: "User registered successful"));
        }
        catch (Exception e)
        {
            _logger.LogError("Error at register: {}", e.Message);
            return HandleException(e, "Internal Server Error");
        }
    }
}