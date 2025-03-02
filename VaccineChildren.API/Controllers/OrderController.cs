using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Application.Services;
using VaccineChildren.Core.Base;

namespace VaccineChildren.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "user")]
public class OrderController : BaseController
{
    private readonly ILogger<OrderController> _logger;
    private readonly IOrderService _orderService;
    private readonly IConfiguration _configuration;

    public OrderController(ILogger<OrderController> logger, IOrderService orderService)
    {
        _logger = logger;
        _orderService = orderService;
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
    [HttpGet]
    public async Task<IActionResult> HandleVnResponse()
    {
        try
        {
            var response = _orderService.HandleVpnResponse(Request.Query);

            string successUrl = _configuration["Vnpay:PaymentBackReturnUrl"];
            return Redirect(successUrl);
        }
        catch (Exception e)
        {
            _logger.LogError("Error at handle vn response caused by {}", e.Message);
            return HandleException(e, nameof(OrderController));
        }
    }

   
}