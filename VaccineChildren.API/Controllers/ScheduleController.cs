using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Application.DTOs.Response;
using VaccineChildren.Application.Services;
using VaccineChildren.Core.Base;
using VaccineChildren.Core.Exceptions;
using VaccineChildren.Domain.Abstraction;

namespace VaccineChildren.API.Controllers;

[Route("api/[controller]")]
[ApiController]
// [Authorize(Roles = "staff")]
public class ScheduleController : BaseController
{
    private readonly ILogger<ScheduleController> _logger;
    private readonly IVaccineScheduleService _vaccineScheduleService;
    private readonly IScheduleService _scheduleService;

    public ScheduleController(
        ILogger<ScheduleController> logger, 
        IVaccineScheduleService vaccineScheduleService,
        IScheduleService scheduleService)
    {
        _logger = logger;
        _vaccineScheduleService = vaccineScheduleService;
        _scheduleService = scheduleService;
    }

    [HttpGet]
    [Authorize(Roles = "staff,doctor")]
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
    [HttpPut("check-in/{scheduleId}")]
    [Authorize(Roles = "staff,doctor")]
    public async Task<IActionResult> UpdateToCheckInStatus([FromRoute] Guid scheduleId)
    {
        try
        {
            var result = await _vaccineScheduleService.UpdateToCheckInStatusAsync(scheduleId);
        
            if (!result)
            {
                return BadRequest(BaseResponse<string>.BadRequestResponse(
                    "Failed to update schedule status. Schedule may not exist or is not in Upcoming status."));
            }
        
            return Ok(BaseResponse<string>.OkResponse("Schedule status updated to Check-in successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError("{Classname} - Error updating schedule status to Check-in: {Message}", 
                nameof(ScheduleController), ex.Message);
            return HandleException(ex, nameof(ScheduleController));
        }
    }
    
    
    [HttpPost]
    public async Task<IActionResult> GenerateTemporarySchedule([FromBody] List<ScheduleReq> requests)
    {
        try
        {
            if (requests == null || !requests.Any())
            {
                throw new ArgumentNullException(nameof(requests), "Request list cannot be null or empty.");
            }

            if (_scheduleService == null)
            {
                _logger.LogError("ScheduleService is not initialized in ScheduleController.");
                throw new InvalidOperationException("ScheduleService is not initialized.");
            }

            _logger.LogInformation("Generating temporary schedules for {Count} requests", requests.Count);

            var schedules = await _scheduleService.GenerateTemporaryScheduleAsync(requests);

            var scheduleResponses = schedules.Select(s => new ScheduleRes
            {
                ChildrenId = s.ChildId ?? Guid.Empty,
                VaccineType = s.VaccineType ?? "Unknown",
                ScheduleDate = s.ScheduleDate?.ToString("yyyy-MM-dd") ?? string.Empty,
                ScheduleStatus = s.IsVaccinated == true ? "Vaccinated" : "Pending"
            }).ToList();

            return Ok(BaseResponse<List<ScheduleRes>>.OkResponse(scheduleResponses,
                "Temporary vaccine schedules generated successfully"));
        }
        catch (Exception e)
        {
            _logger.LogError("{Classname} - Error at generate temporary schedule async caused by {Error}",
                nameof(ScheduleController), e.Message);
            return HandleException(e, nameof(ScheduleController));
        }
    }
}