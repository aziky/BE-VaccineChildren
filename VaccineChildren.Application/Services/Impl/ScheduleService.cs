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

        // Cập nhật để nhận danh sách ScheduleReq và trả về danh sách Schedule
        public async Task<IEnumerable<Schedule>> GenerateTemporaryScheduleAsync(List<ScheduleReq> requests)
        {
            if (requests == null || !requests.Any())
                throw new ArgumentNullException(nameof(requests), "Request list cannot be null or empty.");

            var allSchedules = new List<Schedule>();

            foreach (var request in requests)
            {
                try
                {
                    _logger.LogInformation("Generating temporary schedule for vaccine {VaccineId} and child {ChildId}",
                        request.VaccineId, request.ChildId);

                    if (request.VaccineId == Guid.Empty)
                        throw new ArgumentException("VaccineId cannot be empty.", nameof(request.VaccineId));
                    if (request.ChildId == Guid.Empty)
                        throw new ArgumentException("ChildId cannot be empty.", nameof(request.ChildId));

                    var vaccine = await _vaccineRepository.Entities
                        .AsNoTracking()
                        .FirstOrDefaultAsync(v => v.VaccineId == request.VaccineId);

                    if (vaccine == null)
                    {
                        _logger.LogWarning("Vaccine not found with ID: {VaccineId}", request.VaccineId);
                        throw new CustomExceptions.EntityNotFoundException("Vaccine", request.VaccineId.ToString());
                    }

                    if (!vaccine.IsActive.GetValueOrDefault())
                    {
                        throw new ValidationException("Cannot generate schedule for an inactive vaccine.");
                    }

                    var child = await _childRepository.GetByIdAsync(request.ChildId);
                    if (child == null)
                    {
                        _logger.LogWarning("Child not found with ID: {ChildId}", request.ChildId);
                        throw new CustomExceptions.EntityNotFoundException("Child", request.ChildId.ToString());
                    }

                    var childAgeInMonths = CalculateAgeInMonths(child.Dob, request.StartDate);
                    if (vaccine.MinAge.HasValue && childAgeInMonths < vaccine.MinAge.Value)
                    {
                        throw new ValidationException($"Child's age ({childAgeInMonths} months) is below the minimum age ({vaccine.MinAge.Value} months) required for this vaccine.");
                    }
                    if (vaccine.MaxAge.HasValue && childAgeInMonths > vaccine.MaxAge.Value)
                    {
                        throw new ValidationException($"Child's age ({childAgeInMonths} months) exceeds the maximum age ({vaccine.MaxAge.Value} months) allowed for this vaccine.");
                    }

                    var schedules = GenerateSchedule(vaccine, child, request.StartDate);
                    if (!schedules.Any())
                    {
                        throw new ValidationException("Generated schedule is empty. Check vaccine configuration (NumberDose or Duration).");
                    }

                    // Lưu lịch vào Redis
                    string cacheKey = $"schedule:{request.ChildId}:{request.VaccineId}";
                    await _cacheService.SetAsync(cacheKey, schedules.ToList(), TimeSpan.FromDays(7));

                    allSchedules.AddRange(schedules);
                    _logger.LogInformation("Temporary schedule generated and cached successfully for vaccine {VaccineId}", request.VaccineId);
                }
                catch (Exception e)
                {
                    _logger.LogError("Error generating temporary schedule for vaccine {VaccineId} and child {ChildId}: {Error}",
                        request.VaccineId, request.ChildId, e.Message);
                    throw; // Có thể thay bằng continue nếu muốn bỏ qua lỗi từng request
                }
            }

            return allSchedules;
        }

        // Phương thức cũ để tương thích với trường hợp đơn lẻ
        public async Task<IEnumerable<Schedule>> GenerateTemporaryScheduleAsync(Guid vaccineId, Guid childId, DateTime startDate)
        {
            var request = new List<ScheduleReq> { new ScheduleReq { VaccineId = vaccineId, ChildId = childId, StartDate = startDate } };
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