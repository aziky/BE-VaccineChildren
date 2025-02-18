using Microsoft.Extensions.Logging;
using VaccineChildren.Application.DTOs.Response;
using VaccineChildren.Core.Store;
using VaccineChildren.Domain.Abstraction;
using VaccineChildren.Domain.Entities;

namespace VaccineChildren.Application.Services.Impl;

public class VaccineScheduleService : IVaccineScheduleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<VaccineScheduleService> _logger;

    public VaccineScheduleService(IUnitOfWork unitOfWork, ILogger<VaccineScheduleService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<List<VaccineScheduleRes>> GetVaccineScheduleAsync(DateTime fromDate)
    {
        var scheduleRepository = _unitOfWork.GetRepository<Schedule>();
        var childrenRepository = _unitOfWork.GetRepository<Child>();

        // Get all matching schedules
        var schedules = await scheduleRepository.GetAllAsync();
        var filteredSchedules = schedules.Where(s => s.ScheduleDate >= fromDate || s.ActualDate == null).ToList();

        var result = new List<VaccineScheduleRes>();

        foreach (var schedule in filteredSchedules)
        {
            var child = await childrenRepository.FindByConditionAsync(c => c.ChildId == schedule.ChildId);
            if (child == null) continue;

            var parentName = await _unitOfWork.GetRepository<User>()
                .FindByConditionAsync(u => u.UserId == child.UserId);

            var scheduleStatus = schedule.ActualDate.HasValue
                ? StaticEnum.ScheduleStatusEnum.Completed.ToString()
                : StaticEnum.ScheduleStatusEnum.Upcoming.ToString();

            result.Add(new VaccineScheduleRes
            {
                ChildrenName = child.FullName,
                VaccineName = schedule.VaccineType,
                ScheduleDate = schedule.ScheduleDate,
                ScheduleStatus = scheduleStatus,
                ParentsName = parentName?.FullName ?? "Unknown"
            });
        }

        return result;
    }
}