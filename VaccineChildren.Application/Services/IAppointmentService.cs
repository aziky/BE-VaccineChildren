using VaccineChildren.Application.DTOs.Request;

namespace VaccineChildren.Application.Services;

public interface IAppointmentService
{
     Task CreateAppointmentAsync(CreateAppointmentReq request);
}