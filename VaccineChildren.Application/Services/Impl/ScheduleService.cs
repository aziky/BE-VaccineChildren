using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Core.Exceptions;
using VaccineChildren.Domain.Abstraction;
using VaccineChildren.Domain.Entities;

namespace VaccineChildren.Application.Services.Impl
{
    public class ScheduleService : IScheduleService
    {
        private readonly ILogger<IScheduleService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Vaccine> _vaccineRepository;
        private readonly IGenericRepository<Child> _childRepository;
        private readonly ICacheService _cacheService;

        public ScheduleService(
            ILogger<IScheduleService> logger,
            IUnitOfWork unitOfWork,
            ICacheService cacheService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            _vaccineRepository = _unitOfWork.GetRepository<Vaccine>();
            _childRepository = _unitOfWork.GetRepository<Child>();
        }

        public async Task<IEnumerable<Schedule>> GenerateTemporaryScheduleAsync(List<ScheduleReq> requests)
        {
            if (requests == null || !requests.Any())
                throw new ArgumentNullException(nameof(requests), "Request list cannot be null or empty.");

            var allSchedules = new List<Schedule>();

            // Lấy danh sách ChildId và VaccineId duy nhất từ requests
            var childIds = requests.Select(r => r.ChildId).Distinct().ToList();
            var vaccineIds = requests.SelectMany(r => r.VaccineIds).Distinct().ToList();

            _logger.LogInformation("Pre-fetching data for {ChildCount} children and {VaccineCount} vaccines", childIds.Count, vaccineIds.Count);

            // Tải trước tất cả Child và Vaccine từ DB bằng IUnitOfWork
            var children = await _childRepository.Entities
                .Where(c => childIds.Contains(c.ChildId))
                .ToDictionaryAsync(c => c.ChildId, c => c);

            var vaccines = await _vaccineRepository.Entities
                .AsNoTracking()
                .Where(v => vaccineIds.Contains(v.VaccineId))
                .ToDictionaryAsync(v => v.VaccineId, v => v);

            foreach (var request in requests)
            {
                try
                {
                    if (!children.TryGetValue(request.ChildId, out var child))
                    {
                        _logger.LogWarning("Child not found with ID: {ChildId}", request.ChildId);
                        throw new CustomExceptions.EntityNotFoundException("Child", request.ChildId.ToString());
                    }

                    var childAgeInYears = CalculateAgeInYears(child.Dob, request.StartDate);
                    var requestSchedules = new List<Schedule>();

                    foreach (var vaccineId in request.VaccineIds)
                    {
                        if (!vaccines.TryGetValue(vaccineId, out var vaccine))
                        {
                            _logger.LogWarning("Vaccine not found with ID: {VaccineId}", vaccineId);
                            throw new CustomExceptions.EntityNotFoundException("Vaccine", vaccineId.ToString());
                        }

                        if (!vaccine.IsActive.GetValueOrDefault())
                        {
                            throw new ValidationException($"Vaccine {vaccine.VaccineName} is inactive.");
                        }

                        if (vaccine.MinAge.HasValue && childAgeInYears < vaccine.MinAge.Value)
                        {
                            throw new ValidationException($"Child's age ({childAgeInYears} years) is below the minimum age ({vaccine.MinAge.Value} years) required for vaccine {vaccine.VaccineName}.");
                        }
                        if (vaccine.MaxAge.HasValue && childAgeInYears > vaccine.MaxAge.Value)
                        {
                            throw new ValidationException($"Child's age ({childAgeInYears} years) exceeds the maximum age ({vaccine.MaxAge.Value} years) allowed for vaccine {vaccine.VaccineName}.");
                        }

                        var schedules = GenerateSchedule(vaccine, child, request.StartDate);
                        if (!schedules.Any())
                        {
                            throw new ValidationException($"Generated schedule is empty for vaccine {vaccine.VaccineName}. Check NumberDose or Duration.");
                        }

                        requestSchedules.AddRange(schedules);
                        _logger.LogInformation("Temporary schedule generated for vaccine {VaccineId} and child {ChildId}", vaccineId, request.ChildId);
                    }

                    // Lưu tất cả lịch của request này vào Redis trong một lần
                    string cacheKey = $"schedule:{request.ChildId}";
                    await _cacheService.SetAsync(cacheKey, requestSchedules, TimeSpan.FromDays(7));

                    allSchedules.AddRange(requestSchedules);
                }
                catch (Exception e)
                {
                    _logger.LogError("Error generating temporary schedule for child {ChildId}: {Error}", request.ChildId, e.Message);
                    throw;
                }
            }

            _logger.LogInformation("Generated and cached {ScheduleCount} schedules successfully", allSchedules.Count);
            return allSchedules;
        }

        public async Task<IEnumerable<Schedule>> GenerateTemporaryScheduleAsync(Guid vaccineId, Guid childId, DateTime startDate)
        {
            var request = new List<ScheduleReq>
            {
                new ScheduleReq { VaccineIds = new List<Guid> { vaccineId }, ChildId = childId, StartDate = startDate }
            };
            return await GenerateTemporaryScheduleAsync(request);
        }

        private IEnumerable<Schedule> GenerateSchedule(Vaccine vaccine, Child child, DateTime startDate)
        {
            var scheduleList = new List<Schedule>();
            var numberOfDoses = vaccine.NumberDose.GetValueOrDefault(1);
            var durationInDays = vaccine.Duration.GetValueOrDefault(0);

            if (numberOfDoses <= 0)
            {
                throw new ValidationException("Number of doses must be greater than 0.");
            }

            for (int dose = 1; dose <= numberOfDoses; dose++)
            {
                var scheduleDate = startDate.AddDays((dose - 1) * durationInDays);
                var schedule = new Schedule
                {
                    ScheduleId = Guid.NewGuid(),
                    ChildId = child.ChildId,
                    VaccineType = vaccine.VaccineName,
                    ScheduleDate = scheduleDate,
                    IsVaccinated = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                };
                scheduleList.Add(schedule);
            }

            return scheduleList;
        }

        private double CalculateAgeInYears(DateOnly? dob, DateTime currentDate)
        {
            if (!dob.HasValue) return 0;

            var birthDate = dob.Value.ToDateTime(TimeOnly.MinValue);
            var ageSpan = currentDate - birthDate;
            return Math.Round(ageSpan.TotalDays / 365.25, 1);
        }
    }
}