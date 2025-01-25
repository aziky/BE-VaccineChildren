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
    public async Task<IActionResult> GetAccount()
    {
        try
        {
            // AccountRes accountRes =  await _dashboardService.GetAccountAsync();
            AccountRes accountRes = new AccountRes();
            if (!accountRes.AccountDictionary.Any())
            {
                throw new Exception();
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