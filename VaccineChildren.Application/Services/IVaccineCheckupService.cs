using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Application.DTOs.Response;

namespace VaccineChildren.Application.Services;

public interface IVaccineCheckupService
{
    Task<BaseResponseModel> SavePreVaccineCheckupAsync(PreVaccineCheckupRequest request);
    Task<PreVaccineCheckupResponse> GetPreVaccineCheckupAsync(Guid scheduleId);
}