using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VaccineChildren.Application.DTOs.Response;
using VaccineChildren.Application.Services;
using VaccineChildren.Core.Base;

namespace VaccineChildren.API.Controllers;

[Route("api/[controller]")]
[Authorize("user")]
public class PaymentController : BaseController
{
    private readonly ILogger<PaymentController> _logger;
    private readonly IPaymentService _paymentService;

    public PaymentController(ILogger<PaymentController> logger, 
        IPaymentService paymentService)
    {
        _logger = logger;
        _paymentService = paymentService;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetPaymentsByUserId([FromRoute] Guid userId)
    {
        try
        {
            _logger.LogInformation("Start handle request get payment with user id {}", userId);
            var response = await _paymentService.GetPaymentHistory(userId);
            return Ok(BaseResponse<IList<PaymentHistoryRes>>.OkResponse(response, "Payment history"));
        }
        catch (Exception e)
        {
            _logger.LogError("Error at get payment bu userId cause by {}", e.Message);
            return HandleException(e, nameof(PaymentController));
        }
    }
    
    [HttpGet("details/{orderId}")]
    public async Task<IActionResult> GetVaccinated([FromRoute] Guid orderId)
    {
        try
        {
            _logger.LogInformation("Start handle request get payment with order id {}", orderId);
            var response = await _paymentService.GetVaccinatedHistory(orderId);
            return Ok(BaseResponse<IList<VaccinatedHistory>>.OkResponse(response, "Vaccinated history"));
        }
        catch (Exception e)
        {
            _logger.LogError("Error at get payment bu userId cause by {}", e.Message);
            return HandleException(e, nameof(PaymentController));
        }
    }
}