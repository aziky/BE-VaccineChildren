namespace VaccineChildren.Application.DTOs.Request;

public class PreVaccineCheckupRequest
{
    public Guid ScheduleId { get; set; }
    public Guid DoctorId { get; set; }  // Added DoctorId field (User.UserId)
    
    // Vital signs
    public decimal Weight { get; set; }
    public decimal Height { get; set; }
    public decimal Temperature { get; set; }
    public string BloodPressure { get; set; }
    public int Pulse { get; set; }
    
    // Medical history
    public string[] ChronicDiseases { get; set; }
    public string OtherDiseases { get; set; }
    public string CurrentMedications { get; set; }
    public string PreviousVaccineReactions { get; set; }
    public string MedicalHistory { get; set; }
}