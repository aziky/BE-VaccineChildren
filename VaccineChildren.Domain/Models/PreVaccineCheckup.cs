namespace VaccineChildren.Domain.Entities;

public class PreVaccineCheckup
{
    // Vital signs
    public decimal? Weight { get; set; }
    public decimal? Height { get; set; }
    public decimal? Temperature { get; set; }
    public string? BloodPressure { get; set; }
    public int? Pulse { get; set; }
    
    // Medical history
    public List<string>? ChronicDiseases { get; set; } = new List<string>();
    public string? OtherDiseases { get; set; }
    public string? CurrentMedications { get; set; }
    public string? PreviousVaccineReactions { get; set; }
    public string? MedicalHistory { get; set; }
}