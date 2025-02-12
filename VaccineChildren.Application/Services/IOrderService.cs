using VaccineChildren.Application.DTOs.Request;

namespace VaccineChildren.Application.Services;

public interface IOrderService
{
     Task CreateAppointmentAsync(CreateOrderReq request);
}