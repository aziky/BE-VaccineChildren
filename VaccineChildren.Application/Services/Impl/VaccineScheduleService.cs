using Microsoft.Extensions.Logging;
using VaccineChildren.Application.DTOs.Response;
using VaccineChildren.Core.Exceptions;
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
        try
        {
            _logger.LogInformation("{ClassName} - Getting vaccine schedules", nameof(VaccineScheduleService));
            var scheduleRepository = _unitOfWork.GetRepository<Schedule>();
            var childrenRepository = _unitOfWork.GetRepository<Child>();
            var userRepository = _unitOfWork.GetRepository<User>();
            var result = new List<VaccineScheduleRes>();

            // Get all schedules
            IList<Schedule> allSchedules = await scheduleRepository.GetAllAsync();

            // Filter schedules based on status and date criteria
            var filteredSchedules = allSchedules.Where(s =>
                    // Get "Upcoming" schedules regardless of date
                    (s.status == StaticEnum.ScheduleStatusEnum.Upcoming.Name() && s.ScheduleDate.HasValue) ||
                    // Get other statuses only from the specified date
                    (s.status != StaticEnum.ScheduleStatusEnum.Upcoming.Name() && s.ActualDate.Value >= fromDate)
                )
                .OrderBy(s => s.ScheduleDate)
                .ToList();

            foreach (var schedule in filteredSchedules)
            {
                var child = await childrenRepository.FindByConditionAsync(c => c.ChildId == schedule.ChildId);
                if (child == null) continue;

                var parentName = await userRepository.FindByConditionAsync(u => u.UserId == child.UserId);
                if (parentName == null) continue;

                result.Add(new VaccineScheduleRes
                {
                    ScheduleId = schedule.ScheduleId,
                    ChildrenName = child.FullName,
                    VaccineId = schedule.VaccineId,
                    ScheduleDate = string.Format("{0:dd-MM-yyyy HH:mm}", schedule.ScheduleDate),
                    ScheduleStatus = schedule.status,
                    ParentsName = parentName?.FullName,
                    PhoneNumber = parentName?.Phone
                });
            }

            if (!result.Any())
            {
                throw new CustomExceptions.NoDataFoundException("No vaccine schedules found");
            }

            _logger.LogInformation("Getting vaccine schedules done");
            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(
                $"{nameof(VaccineScheduleService)} - Error at get vaccine schedule async cause by: {e.Message}");
            throw;
        }
    }

    public async Task<bool> UpdateToCheckInStatusAsync(Guid scheduleId)
    {
        try
        {
            _logger.LogInformation("{ClassName} - Updating schedule status to Check-in for schedule {ScheduleId}",
                nameof(VaccineScheduleService), scheduleId);

            var scheduleRepository = _unitOfWork.GetRepository<Schedule>();
            var schedule = await scheduleRepository.FindByConditionAsync(s => s.ScheduleId == scheduleId);

            if (schedule == null)
            {
                _logger.LogWarning("Schedule not found: {ScheduleId}", scheduleId);
                return false;
            }

            if (schedule.status != StaticEnum.ScheduleStatusEnum.Upcoming.Name())
            {
                _logger.LogWarning("Cannot update status to Check-in. Schedule {ScheduleId} is not in Upcoming status",
                    scheduleId);
                return false;
            }

            // Update the status
            schedule.status = StaticEnum.ScheduleStatusEnum.CheckIn.Name();
            schedule.UpdatedAt = DateTime.UtcNow.ToLocalTime();
            schedule.ActualDate = schedule.UpdatedAt;

            await _unitOfWork.SaveChangeAsync();

            _logger.LogInformation("Successfully updated schedule {ScheduleId} status to Check-in", scheduleId);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(
                "{ClassName} - Error updating schedule status to Check-in for schedule {ScheduleId}: {Message}",
                nameof(VaccineScheduleService), scheduleId, e.Message);
            throw;
        }
    }
}