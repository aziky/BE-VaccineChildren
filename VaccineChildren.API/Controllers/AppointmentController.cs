using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Application.Services;
using VaccineChildren.Core.Base;

namespace VaccineChildren.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize (Roles = "user")]
public class AppointmentController :  BaseController
{
    private readonly ILogger<AppointmentController> _logger;
    private readonly IAppointmentService _appointmentService;

    public AppointmentController(ILogger<AppointmentController> logger, IAppointmentService appointmentService)
    {
        _logger = logger;
        _appointmentService = appointmentService;
    }

    [HttpPost("appointment")]
    public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentReq request)
    {
        try
        {
            await _appointmentService.CreateAppointmentAsync(request);
            return Ok(BaseResponse<string>.CreateResponse("Appointment created"));
        }
        catch (Exception e)
        {
            _logger.LogError("Error at create appointment cause by {}", e.Message);
            return HandleException(e, nameof(AppointmentController));
        }
    }
}