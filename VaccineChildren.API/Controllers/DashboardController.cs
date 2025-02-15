using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VaccineChildren.Application.DTOs.Response;
using VaccineChildren.Application.Services;
using VaccineChildren.Core.Base;
using VaccineChildren.Core.Exceptions;
using VaccineChildren.Core.Store;

namespace VaccineChildren.API.Controllers;

[Route("api/[controller]")]
[ApiController]
// [Authorize (Roles = "admin")]
public class DashboardController : BaseController
{
    private readonly ILogger<DashboardController> _logger;
    private readonly IDashboardService _dashboardService;

    public DashboardController(ILogger<DashboardController> logger, IDashboardService dashboardService)
    {
        _logger = logger;
        _dashboardService = dashboardService;
    }

    [HttpGet("account")]
    public async Task<IActionResult> GetAccount([FromQuery] int year)
    {
        try
        {
            AccountRes accountRes =  await _dashboardService.GetAccountAsync(year);
            if (!accountRes.AccountDictionary.Any())
            {
                throw new CustomExceptions.NoDataFoundException("There's no account");
            }
            return Ok(BaseResponse<AccountRes>.OkResponse(accountRes, "Get account successful"));
        }
        catch (Exception e)
        {
            _logger.LogError("{Classname} - Error at get account async cause by {}", nameof(DashboardController), e.Message);
            return HandleException(e, nameof(DashboardController));
        }
    }

    [HttpGet("revenue")]
    public async Task<IActionResult> GetRevenue([FromQuery] int year)
    {
        try
        {
            var revenueDataRes = await _dashboardService.GetRevenueDataAsync(year);
            return Ok(BaseResponse<RevenueDataRes>.OkResponse(revenueDataRes, "Get Revenue data successful"));
        }
        catch (Exception e)
        {
            _logger.LogError($"{nameof(DashboardController)} - Error at get account async cause by {e.Message}");
            return HandleException(e, nameof(DashboardController));
        }
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetDashboardSummaryAsync([FromQuery] int year)
    {
        try
        {
            var dashboardSummaryRes = await _dashboardService.GetDashboardSummaryAsync(year);
            return Ok(BaseResponse<DashboardSummaryRes>.OkResponse(dashboardSummaryRes, "Get Vaccine dashboard successful"));
        }
        catch (Exception e)
        {
            _logger.LogError($"{nameof(DashboardController)} - Error at get dashboard summary async cause by {e.Message}");
            return HandleException(e, nameof(DashboardController));
        }
    }
}