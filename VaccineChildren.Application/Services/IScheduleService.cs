using VaccineChildren.Domain.Entities;
using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Application.DTOs.Response;

namespace VaccineChildren.Domain.Abstraction
{
    public interface IScheduleService
    {
        Task<IEnumerable<ScheduleRes>> GenerateTemporaryScheduleAsync(List<ScheduleReq> requests);
        Task<IEnumerable<ScheduleRes>> GenerateTemporaryScheduleAsync(Guid vaccineId, Guid childId, DateTime startDate);
    }
}