using Microsoft.AspNetCore.Mvc;
using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Application.Services;
using VaccineChildren.Core.Base;

namespace VaccineChildren.API.Controllers;

[Route("api/[controller]")]
[ApiController]
// [Authorize (Roles = "user")]
public class OrderController :  BaseController
{
    private readonly ILogger<OrderController> _logger;
    private readonly IOrderService _orderService;

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
            await _orderService.CreateOrderAsync(request);
            return Ok(BaseResponse<string>.CreateResponse("Order created"));
        }
        catch (Exception e)
        {
            _logger.LogError("Error at create order cause by {}", e.Message);
            return HandleException(e, nameof(OrderController));
        }
    }
}