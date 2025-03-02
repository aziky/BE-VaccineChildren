using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Application.Services;
using VaccineChildren.Core.Base;
using Swashbuckle.AspNetCore.Annotations;

namespace VaccineChildren.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "user")]
public class OrderController : BaseController
{
    private readonly ILogger<OrderController> _logger;
    private readonly IOrderService _orderService;
    private readonly IConfiguration _configuration;

    public OrderController(ILogger<OrderController> logger, IOrderService orderService, IConfiguration configuration)
    {
        _logger = logger;
        _orderService = orderService;
        _configuration = configuration;
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
    [SwaggerOperation(Summary = "Handle VNPAY Payment Response", 
        Description = "Processes the payment response from VNPAY.")]
    public async Task<IActionResult> HandleVnResponse()
    {
        try
        {
            var response = await _orderService.HandleVnPayResponse(Request.Query);
            if (response == false)
            {
                return Redirect(_configuration["PaymentCancelUrl"]);
            }
            string successUrl = _configuration["PaymentSuccessUrl"];
            return Redirect(successUrl);
        }
        catch (Exception e)
        {
            _logger.LogError("Error at handle vn response caused by {}", e.Message);
            return HandleException(e, nameof(OrderController));
        }
    }

   
}