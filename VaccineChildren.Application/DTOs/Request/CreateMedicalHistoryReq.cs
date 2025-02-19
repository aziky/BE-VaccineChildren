namespace VaccineChildren.Application.DTOs.Request;

public class CreateMedicalHistoryReq
{
    public string UserId { get; set; }
    public string ScheduleId { get; set; }
    public double Weight { get; set; }
    public double Height { get; set; }
    public double Temperature { get; set; }
    public string BloodPressure { get; set; }
    public int HeartRate { get; set; }
    public MedicalHistoryDetails MedicalHistory { get; set; }
}

public class MedicalHistoryDetails
{
    public bool? HasDiabetes { get; set; }
    public bool? HasAsthma { get; set; }
    public bool? HasHeartDisease { get; set; }
    public bool? HasAutoImmuneDisease { get; set; }
    public string OtherDiseases { get; set; }
    public string CurrentMedications { get; set; }
    public string PreviousVaccineReactions { get; set; }
    public string AdditionalNotes { get; set; }
}