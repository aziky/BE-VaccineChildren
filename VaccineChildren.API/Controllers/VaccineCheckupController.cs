using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Application.DTOs.Response;
using VaccineChildren.Application.Services;
using VaccineChildren.Core.Base;

namespace VaccineChildren.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "doctor")]
public class VaccineCheckupController : BaseController
{
    private readonly IVaccineCheckupService _vaccineCheckupService;
    private readonly ILogger<VaccineCheckupController> _logger;

    public VaccineCheckupController(
        IVaccineCheckupService vaccineCheckupService,
        ILogger<VaccineCheckupController> logger)
    {
        _vaccineCheckupService = vaccineCheckupService;
        _logger = logger;
    }

    [HttpPost("pre-vaccine")]
    public async Task<IActionResult> SavePreVaccineCheckup([FromBody] PreVaccineCheckupRequest request)
    {
        try
        {
            var result = await _vaccineCheckupService.SavePreVaccineCheckupAsync(request);
            
            if (!result.Success)
            {
                return BadRequest(BaseResponse<string>.BadRequestResponse(result.Message));
            }
            
            return Ok(BaseResponse<string>.OkResponse(result.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving pre-vaccine checkup data");
            return HandleException(ex, nameof(VaccineCheckupController));
        }
    }

    [HttpGet("pre-vaccine/{scheduleId}")]
    public async Task<IActionResult> GetPreVaccineCheckup([FromRoute] Guid scheduleId)
    {
        try
        {
            var result = await _vaccineCheckupService.GetPreVaccineCheckupAsync(scheduleId);
            
            if (result == null)
            {
                return NotFound(BaseResponse<string>.BadRequestResponse("Pre-vaccine checkup data not found"));
            }
            
            return Ok(BaseResponse<PreVaccineCheckupResponse>.OkResponse(result, "Pre-vaccine checkup data retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pre-vaccine checkup data");
            return HandleException(ex, nameof(VaccineCheckupController));
        }
    }
}