using VaccineChildren.Domain.Entities;
using VaccineChildren.Application.DTOs.Request;

namespace VaccineChildren.Domain.Abstraction
{
    public interface IScheduleService
    {
        Task<IEnumerable<Schedule>> GenerateTemporaryScheduleAsync(Guid vaccineId, Guid childId, DateTime startDate);
        Task<IEnumerable<Schedule>> GenerateTemporaryScheduleAsync(List<ScheduleReq> requests); // Thêm phương thức mới
    }
}