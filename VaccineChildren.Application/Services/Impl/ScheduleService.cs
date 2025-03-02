using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
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
            ICacheService cacheService) // Thêm ICacheService vào constructor
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            _vaccineRepository = _unitOfWork.GetRepository<Vaccine>();
            _childRepository = _unitOfWork.GetRepository<Child>();
        }

        public async Task<IEnumerable<Schedule>> GenerateTemporaryScheduleAsync(Guid vaccineId, Guid childId, DateTime startDate)
        {
            try
            {
                _logger.LogInformation("Generating temporary schedule for vaccine {VaccineId} and child {ChildId}", 
                    vaccineId, childId);

                if (vaccineId == Guid.Empty)
                    throw new ArgumentException("VaccineId cannot be empty.");
                if (childId == Guid.Empty)
                    throw new ArgumentException("ChildId cannot be empty.");

                if (_vaccineRepository == null)
                {
                    _logger.LogError("VaccineRepository is null.");
                    throw new InvalidOperationException("VaccineRepository is not initialized.");
                }

                if (_childRepository == null)
                {
                    _logger.LogError("ChildRepository is null.");
                    throw new InvalidOperationException("ChildRepository is not initialized.");
                }

                var vaccine = await _vaccineRepository.Entities
                    .AsNoTracking()
                    .FirstOrDefaultAsync(v => v.VaccineId == vaccineId);

                if (vaccine == null)
                {
                    _logger.LogWarning("Vaccine not found with ID: {VaccineId}", vaccineId);
                    throw new CustomExceptions.EntityNotFoundException("Vaccine", vaccineId.ToString());
                }

                if (!vaccine.IsActive.GetValueOrDefault())
                {
                    throw new ValidationException("Cannot generate schedule for an inactive vaccine.");
                }

                var child = await _childRepository.GetByIdAsync(childId);
                if (child == null)
                {
                    _logger.LogWarning("Child not found with ID: {ChildId}", childId);
                    throw new CustomExceptions.EntityNotFoundException("Child", childId.ToString());
                }

                var childAgeInMonths = CalculateAgeInMonths(child.Dob, startDate);
                if (vaccine.MinAge.HasValue && childAgeInMonths < vaccine.MinAge.Value)
                {
                    throw new ValidationException($"Child's age ({childAgeInMonths} months) is below the minimum age ({vaccine.MinAge.Value} months) required for this vaccine.");
                }
                if (vaccine.MaxAge.HasValue && childAgeInMonths > vaccine.MaxAge.Value)
                {
                    throw new ValidationException($"Child's age ({childAgeInMonths} months) exceeds the maximum age ({vaccine.MaxAge.Value} months) allowed for this vaccine.");
                }

                var schedule = GenerateSchedule(vaccine, child, startDate);

                if (!schedule.Any())
                {
                    throw new ValidationException("Generated schedule is empty. Check vaccine configuration (NumberDose or Duration).");
                }

                // Lưu lịch vào Redis
                string cacheKey = $"schedule:{childId}:{vaccineId}";
                await _cacheService.SetAsync(cacheKey, schedule.ToList(), TimeSpan.FromDays(7)); // Lưu trong 7 ngày

                _logger.LogInformation("Temporary schedule generated and cached successfully for vaccine {VaccineId}", vaccineId);
                return schedule;
            }
            catch (Exception e)
            {
                _logger.LogError("Error generating temporary schedule for vaccine {VaccineId} and child {ChildId}: {Error}", 
                    vaccineId, childId, e.Message);
                throw;
            }
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

        private int CalculateAgeInMonths(DateOnly? dob, DateTime currentDate)
        {
            if (!dob.HasValue) return 0;

            var birthDate = dob.Value;
            var years = currentDate.Year - birthDate.Year;
            var months = currentDate.Month - birthDate.Month;

            if (currentDate.Day < birthDate.Day)
            {
                months--;
            }

            if (months < 0)
            {
                years--;
                months += 12;
            }

            return years * 12 + months;
        }
    }
}