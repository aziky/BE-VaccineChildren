using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Application.DTOs.Response;
using VaccineChildren.Core.Exceptions;
using VaccineChildren.Domain.Abstraction;
using VaccineChildren.Domain.Entities;

namespace VaccineChildren.Application.Services.Impl
{
    public class ScheduleService : IScheduleService
    {
        private readonly ILogger<ScheduleService> _logger; // Sử dụng cụ thể thay vì generic interface
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Vaccine> _vaccineRepository;
        private readonly IGenericRepository<Child> _childRepository;
        private readonly ICacheService _cacheService;

        public ScheduleService(
            ILogger<ScheduleService> logger,
            IUnitOfWork unitOfWork,
            ICacheService cacheService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            _vaccineRepository = _unitOfWork.GetRepository<Vaccine>();
            _childRepository = _unitOfWork.GetRepository<Child>();
        }

        public async Task<IEnumerable<ScheduleRes>> GenerateTemporaryScheduleAsync(List<ScheduleReq> requests)
        {
            ValidateRequests(requests);

            var allSchedules = new List<Schedule>();
            var (children, vaccines) = await FetchDataAsync(requests);

            foreach (var request in requests)
            {
                allSchedules.AddRange(await ProcessRequestAsync(request, children, vaccines));
            }

            // Convert sang ScheduleRes chỉ với VaccineId
            var response = allSchedules.Select(s => new ScheduleRes
            {
                ChildId = s.ChildId.Value,
                VaccineId = vaccines.First(v => v.Value.VaccineId == s.VaccineId).Key,
                ScheduleDate = s.ScheduleDate?.ToString("yyyy-MM-dd") ?? ""
            });

            string cacheKey = $"schedules:batch:{Guid.NewGuid()}";
            await _cacheService.SetAsync(cacheKey, allSchedules, TimeSpan.FromHours(1));

            _logger.LogInformation("Generated and cached {ScheduleCount} schedules successfully with key {CacheKey}", 
                allSchedules.Count, cacheKey);

            return response;
        }

        public async Task<IEnumerable<ScheduleRes>> GenerateTemporaryScheduleAsync(Guid vaccineId, Guid childId, DateTime startDate)
        {
            var request = new ScheduleReq 
            { 
                VaccineIds = new List<Guid> { vaccineId }, 
                ChildId = childId, 
                StartDate = startDate 
            };
            return await GenerateTemporaryScheduleAsync(new List<ScheduleReq> { request });
        }

        private void ValidateRequests(List<ScheduleReq> requests)
        {
            if (requests == null || !requests.Any())
                throw new ArgumentNullException(nameof(requests), "Request list cannot be null or empty.");
        }

        private async Task<(Dictionary<Guid, Child>, Dictionary<Guid, Vaccine>)> FetchDataAsync(List<ScheduleReq> requests)
        {
            var childIds = requests.Select(r => r.ChildId).Distinct().ToList();
            var vaccineIds = requests.SelectMany(r => r.VaccineIds).Distinct().ToList();

            _logger.LogInformation("Pre-fetching data for {ChildCount} children and {VaccineCount} vaccines", childIds.Count, vaccineIds.Count);

            var children = await _childRepository.Entities
                .Where(c => childIds.Contains(c.ChildId))
                .ToDictionaryAsync(c => c.ChildId);

            var vaccines = await _vaccineRepository.Entities
                .AsNoTracking()
                .Where(v => vaccineIds.Contains(v.VaccineId))
                .ToDictionaryAsync(v => v.VaccineId);

            return (children, vaccines);
        }

        private async Task<List<Schedule>> ProcessRequestAsync(ScheduleReq request, 
            Dictionary<Guid, Child> children, 
            Dictionary<Guid, Vaccine> vaccines)
        {
            var child = GetChild(children, request.ChildId);
            var childAgeInMonths = CalculateAgeInMonths(child.Dob, request.StartDate);
            var requestSchedules = new List<Schedule>();

            foreach (var vaccineId in request.VaccineIds)
            {
                var vaccine = GetVaccine(vaccines, vaccineId);
                ValidateVaccine(vaccine, childAgeInMonths);
            
                var schedules = GenerateSchedule(vaccine, child, request.StartDate, vaccineId);
                requestSchedules.AddRange(schedules);
            
                _logger.LogInformation("Temporary schedule generated for vaccine {VaccineId} and child {ChildId}", 
                    vaccineId, request.ChildId);
            }

            string cacheKey = $"schedule:{request.ChildId}:{request.StartDate:yyyyMMddHHmmss}";
            await _cacheService.SetAsync(cacheKey, requestSchedules, TimeSpan.FromMinutes(30));

            return requestSchedules;
        }

        private Child GetChild(Dictionary<Guid, Child> children, Guid childId)
        {
            if (!children.TryGetValue(childId, out var child))
            {
                _logger.LogWarning("Child not found with ID: {ChildId}", childId);
                throw new CustomExceptions.EntityNotFoundException("Child", childId.ToString());
            }
            return child;
        }

        private Vaccine GetVaccine(Dictionary<Guid, Vaccine> vaccines, Guid vaccineId)
        {
            if (!vaccines.TryGetValue(vaccineId, out var vaccine))
            {
                _logger.LogWarning("Vaccine not found with ID: {VaccineId}", vaccineId);
                throw new CustomExceptions.EntityNotFoundException("Vaccine", vaccineId.ToString());
            }
            return vaccine;
        }

        private void ValidateVaccine(Vaccine vaccine, double childAgeInMonths)
        {
            if (!vaccine.IsActive.GetValueOrDefault())
                throw new ValidationException($"Vaccine {vaccine.VaccineName} is inactive.");

            if (vaccine.MinAge.HasValue && childAgeInMonths < vaccine.MinAge.Value)
                throw new ValidationException($"Child's age ({childAgeInMonths} months) is below the minimum age ({vaccine.MinAge.Value} months) required for vaccine {vaccine.VaccineName}.");

            if (vaccine.MaxAge.HasValue && childAgeInMonths > vaccine.MaxAge.Value)
                throw new ValidationException($"Child's age ({childAgeInMonths} months) exceeds the maximum age ({vaccine.MaxAge.Value} months) allowed for vaccine {vaccine.VaccineName}.");
        }

        private IEnumerable<Schedule> GenerateSchedule(Vaccine vaccine, Child child, DateTime startDate, Guid vaccineId)
        {
            var numberOfDoses = vaccine.NumberDose.GetValueOrDefault(1);
            var durationInDays = vaccine.Duration.GetValueOrDefault(0);

            if (numberOfDoses <= 0)
                throw new ValidationException("Number of doses must be greater than 0.");

            return Enumerable.Range(1, numberOfDoses).Select(dose => new Schedule
            {
                ScheduleId = Guid.NewGuid(),
                ChildId = child.ChildId,
                VaccineId = vaccine.VaccineId, // Giữ lại để mapping, có thể thêm VaccineId nếu cần
                ScheduleDate = startDate.AddDays((dose - 1) * durationInDays),
                IsVaccinated = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            });
        }

        private double CalculateAgeInMonths(DateOnly? dob, DateTime currentDate)
        {
            if (!dob.HasValue) return 0;
            var birthDate = dob.Value.ToDateTime(TimeOnly.MinValue);
            return Math.Round((currentDate - birthDate).TotalDays / 30.44, 1);
        }
    }
}