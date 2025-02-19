
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
// [Authorize(Roles = "staff")]
public class ScheduleController : BaseController
{
    private readonly ILogger<ScheduleController> _logger;
    private readonly IVaccineScheduleService _vaccineScheduleService;

    public ScheduleController(
        ILogger<ScheduleController> logger, 
        IVaccineScheduleService vaccineScheduleService)
    {
        _logger = logger;
        _vaccineScheduleService = vaccineScheduleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetVaccineSchedule([FromQuery] DateTime fromDate)
    {
        try
        {
            List<VaccineScheduleRes> schedules = await _vaccineScheduleService.GetVaccineScheduleAsync(fromDate);
            if (!schedules.Any())
            {
                throw new CustomExceptions.NoDataFoundException("There's no schedule");
            }
            return Ok(BaseResponse<List<VaccineScheduleRes>>.OkResponse(schedules, "Get vaccine schedules successful"));
        }
        catch (Exception e)
        {
            _logger.LogError("{Classname} - Error at get vaccine schedule async cause by {}", 
                nameof(ScheduleController), e.Message);
            return HandleException(e, nameof(ScheduleController));
        }
    }
}