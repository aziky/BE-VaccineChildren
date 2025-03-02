using Microsoft.AspNetCore.Http;
using VaccineChildren.Application.DTOs.Request;

namespace VaccineChildren.Application.Services;

public interface IOrderService
{
     Task<string> CreateOrderAsync(CreateOrderReq request, HttpContext httpContext);

     Task HandleVpnResponse(IQueryCollection collection);
}