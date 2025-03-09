using System.Text.Json;
using Microsoft.Extensions.Logging;
using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Application.DTOs.Response;
using VaccineChildren.Core.Store;
using VaccineChildren.Domain.Abstraction;
using VaccineChildren.Domain.Entities;

namespace VaccineChildren.Application.Services.Impl;

public class VaccineCheckupService : IVaccineCheckupService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<VaccineCheckupService> _logger;

    public VaccineCheckupService(
        IUnitOfWork unitOfWork,
        ILogger<VaccineCheckupService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

public async Task<BaseResponseModel> SavePreVaccineCheckupAsync(PreVaccineCheckupRequest request)
{
    try
    {
        _logger.LogInformation("Saving pre-vaccine checkup for schedule {ScheduleId}", request.ScheduleId);
        
        var scheduleRepository = _unitOfWork.GetRepository<Schedule>();
        var schedule = await scheduleRepository.FindByConditionAsync(s => s.ScheduleId == request.ScheduleId);
        
        if (schedule == null)
        {
            _logger.LogWarning("Schedule not found: {ScheduleId}", request.ScheduleId);
            return new BaseResponseModel
            {
                Success = false,
                Message = "Schedule not found"
            };
        }

        // Create the pre-vaccine checkup model
        var checkupData = new PreVaccineCheckup
        {
            Weight = request.Weight,
            Height = request.Height,
            Temperature = request.Temperature,
            BloodPressure = request.BloodPressure,
            Pulse = request.Pulse,
            ChronicDiseases = request.ChronicDiseases?.ToList() ?? new List<string>(),
            OtherDiseases = request.OtherDiseases,
            CurrentMedications = request.CurrentMedications,
            PreviousVaccineReactions = request.PreviousVaccineReactions,
            MedicalHistory = request.MedicalHistory
        };
        
        // Serialize to JSON
        schedule.PreVaccineCheckup = JsonSerializer.Serialize(checkupData, new JsonSerializerOptions
        {
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        
        // Set the doctor who performed the checkup
        schedule.AdministeredBy = request.DoctorId;
        
        schedule.UpdatedAt = DateTime.UtcNow.ToLocalTime();
        // schedule.UpdatedBy = schedule.AdministeredBy.;
        
        await scheduleRepository.UpdateAsync(schedule);
        await _unitOfWork.SaveChangeAsync();
        
        _logger.LogInformation("Pre-vaccine checkup saved successfully for schedule {ScheduleId}", request.ScheduleId);
        
        return new BaseResponseModel
        {
            Success = true,
            Message = "Pre-vaccine checkup data saved successfully"
        };
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error saving pre-vaccine checkup data for schedule {ScheduleId}", request.ScheduleId);
        return new BaseResponseModel
        {
            Success = false,
            Message = "An error occurred while saving pre-vaccine checkup data"
        };
    }
}

    public async Task<PreVaccineCheckupResponse> GetPreVaccineCheckupAsync(Guid scheduleId)
    {
        try
        {
            _logger.LogInformation("Getting pre-vaccine checkup for schedule {ScheduleId}", scheduleId);
            
            var scheduleRepository = _unitOfWork.GetRepository<Schedule>();
            var childRepository = _unitOfWork.GetRepository<Child>();
            
            var schedule = await scheduleRepository.FindByConditionAsync(s => s.ScheduleId == scheduleId);
            
            if (schedule == null)
            {
                _logger.LogWarning("Schedule not found: {ScheduleId}", scheduleId);
                return null;
            }
            
            if (string.IsNullOrEmpty(schedule.PreVaccineCheckup))
            {
                _logger.LogWarning("Pre-vaccine checkup data not found for schedule {ScheduleId}", scheduleId);
                return new PreVaccineCheckupResponse
                {
                    ScheduleId = scheduleId,
                    VaccineId = schedule.VaccineId,
                    ChronicDiseases = new List<string>()
                };
            }
            
            var child = await childRepository.FindByConditionAsync(c => c.ChildId == schedule.ChildId);
            
            var checkupData = JsonSerializer.Deserialize<PreVaccineCheckup>(schedule.PreVaccineCheckup, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            return new PreVaccineCheckupResponse
            {
                ScheduleId = scheduleId,
                ChildName = child?.FullName ?? "Unknown",
                VaccineId = schedule.VaccineId,
                Weight = checkupData.Weight,
                Height = checkupData.Height,
                Temperature = checkupData.Temperature,
                BloodPressure = checkupData.BloodPressure,
                Pulse = checkupData.Pulse,
                ChronicDiseases = checkupData.ChronicDiseases ?? new List<string>(),
                OtherDiseases = checkupData.OtherDiseases,
                CurrentMedications = checkupData.CurrentMedications,
                PreviousVaccineReactions = checkupData.PreviousVaccineReactions,
                MedicalHistory = checkupData.MedicalHistory
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pre-vaccine checkup data for schedule {ScheduleId}", scheduleId);
            throw;
        }
    }
}