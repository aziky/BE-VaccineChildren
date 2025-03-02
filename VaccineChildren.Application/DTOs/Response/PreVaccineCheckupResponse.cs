namespace VaccineChildren.Application.DTOs.Response;

public class PreVaccineCheckupResponse
{
    public Guid ScheduleId { get; set; }
    public string ChildName { get; set; }
    public string VaccineType { get; set; }
    
    // Vital signs
    public decimal? Weight { get; set; }
    public decimal? Height { get; set; }
    public decimal? Temperature { get; set; }
    public string? BloodPressure { get; set; }
    public int? Pulse { get; set; }
    
    // Medical history
    public List<string>? ChronicDiseases { get; set; }
    public string? OtherDiseases { get; set; }
    public string? CurrentMedications { get; set; }
    public string? PreviousVaccineReactions { get; set; }
    public string? MedicalHistory { get; set; }
}

public class BaseResponseModel
{
    public bool Success { get; set; }
    public string Message { get; set; }
}