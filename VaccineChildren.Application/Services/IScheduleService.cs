using VaccineChildren.Domain.Entities;

namespace VaccineChildren.Domain.Abstraction
{
    public interface IScheduleService
    {
        Task<IEnumerable<Schedule>> GenerateTemporaryScheduleAsync(Guid vaccineId, Guid childId, DateTime startDate);
    }
}