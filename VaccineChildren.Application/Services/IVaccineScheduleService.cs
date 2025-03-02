using VaccineChildren.Application.DTOs.Response;

namespace VaccineChildren.Application.Services;

public interface IVaccineScheduleService
{
    Task<List<VaccineScheduleRes>> GetVaccineScheduleAsync(DateTime fromDate);
    Task<bool> UpdateToCheckInStatusAsync(Guid scheduleId);
}