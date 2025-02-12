using VaccineChildren.Application.DTOs.Request;

namespace VaccineChildren.Application.Services;

public interface IOrderService
{
     Task CreateOrderAsync(CreateOrderReq request);
}