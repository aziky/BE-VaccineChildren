using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Application.DTOs.Response;
using VaccineChildren.Core.Base;

namespace VaccineChildren.API.Controllers;

[Route("api/[controller]")]
[ApiController]
// [Authorize(Roles = "doctor,staff")]
public class VaccineReactionController : BaseController
{
    private readonly IVaccineReactionService _reactionService;
    private readonly ILogger<VaccineReactionController> _logger;

    public VaccineReactionController(
        IVaccineReactionService reactionService,
        ILogger<VaccineReactionController> logger)
    {
        _reactionService = reactionService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> RecordVaccineReaction([FromBody] VaccineReactionRequest request)
    {
        try
        {
            var result = await _reactionService.RecordVaccineReactionAsync(request);
            return Ok(BaseResponse<VaccineReactionResponse>.OkResponse(result, "Vaccine reaction recorded successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording vaccine reaction");
            return HandleException(ex, nameof(VaccineReactionController));
        }
    }

    // [HttpGet("{scheduleId}")]
    // public async Task<IActionResult> GetVaccineReactionsForSchedule([FromRoute] Guid scheduleId)
    // {
    //     try
    //     {
    //         var result = await _reactionService.GetVaccineReactionsForScheduleAsync(scheduleId);
    //         return Ok(BaseResponse<List<VaccineReactionResponse>>.OkResponse(result, "Vaccine reactions retrieved successfully"));
    //     }
    //     catch (NotFoundException ex)
    //     {
    //         return NotFound(BaseResponse<string>.FailResponse(ex.Message));
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Error getting vaccine reactions");
    //         return HandleException(ex, nameof(VaccineReactionController));
    //     }
    // }
}