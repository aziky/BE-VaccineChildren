using Microsoft.AspNetCore.Mvc;
using VaccineChildren.Application.DTOs.Response;
using VaccineChildren.Application.Services;
using VaccineChildren.Core.Base;

namespace VaccineChildren.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DashboardController : BaseController
{
    private readonly ILogger<DashboardController> _logger;
    private readonly IDashboardService _dashboardService;

    public DashboardController(ILogger<DashboardController> logger, IDashboardService dashboardService)
    {
        _logger = logger;
        _dashboardService = dashboardService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAccount([FromQuery] int year)
    {
        try
        {
            AccountRes accountRes =  await _dashboardService.GetAccountAsync(year);
            if (!accountRes.AccountDictionary.Any())
            {
                throw new KeyNotFoundException("There's no account");
            }
            return Ok(BaseResponse<AccountRes>.OkResponse(accountRes, "Get account successful"));
        }
        catch (Exception e)
        {
            _logger.LogError("{Classname} - Error at get account async cause by {}", nameof(DashboardController), e.Message);
            return HandleException(e, nameof(DashboardController));
        }
    }
}