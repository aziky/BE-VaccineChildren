using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Application.Services;
using VaccineChildren.Core.Base;
using Swashbuckle.AspNetCore.Annotations;
using VaccineChildren.Infrastructure.Configuration;

namespace VaccineChildren.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "user")]
public class OrderController : BaseController
{
    private readonly ILogger<OrderController> _logger;
    private readonly IOrderService _orderService;
    private readonly PaymentFrontEndUrl _paymentReturnUrl;
    private readonly IConfiguration _configuration;
    private readonly string _frontendUrl;  // Store FrontendUrl value
    
    public OrderController(ILogger<OrderController> logger, IOrderService orderService,
        PaymentFrontEndUrl paymentReturnUrl, IConfiguration configuration)
    {
        _logger = logger;
        _orderService = orderService;
        _paymentReturnUrl = paymentReturnUrl;
        _configuration = configuration;
        _frontendUrl = _configuration.GetValue<string>("FrontendUrl");
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderReq request)
    {
        try
        {
            return Ok(BaseResponse<string>.OkResponse(data: await _orderService.CreateOrderAsync(request, HttpContext),
                mess: "Order created"));
        }
        catch (Exception e)
        {
            _logger.LogError("Error at create order cause by {}", e.Message);
            return HandleException(e, nameof(OrderController));
        }
    }
    
    [AllowAnonymous]
    [HttpGet("vnpay")]
    [SwaggerOperation(Summary = "Handle VNPAY Payment Response", 
        Description = "Processes the payment response from VNPAY.")]
    public async Task<IActionResult> HandleVnResponse()
    {
        try
        {
            var response = await _orderService.HandleVnPayResponse(Request.Query);
            if (response == false)
            {
                return Redirect(_frontendUrl + _paymentReturnUrl.PaymentCancelUrl);
            }
            string successUrl = _frontendUrl + _paymentReturnUrl.PaymentSuccessUrl;
            return Redirect(successUrl);
        }
        catch (Exception e)
        {
            _logger.LogError("Error at handle vn response caused by {}", e.Message);
            return HandleException(e, nameof(OrderController));
        }
    }
    
    
    [AllowAnonymous]
    [HttpGet("momo")]
    [SwaggerOperation(Summary = "Handle Momo Payment Response", 
        Description = "Processes the payment response from Momo.")]
    public async Task<IActionResult> HandleMomResponse()
    {
        try
        {
            var response = await _orderService.HandleMomoResponse(Request.Query);
            if (response == false)
            {
                return Redirect(_frontendUrl +  _paymentReturnUrl.PaymentCancelUrl);
            }
            string successUrl = _frontendUrl + _paymentReturnUrl.PaymentSuccessUrl;
            return Redirect(successUrl);
        }
        catch (Exception e)
        {
            _logger.LogError("Error at handle momo response caused by {}", e.Message);
            return HandleException(e, nameof(OrderController));
        }
    }
    

   
}