namespace VaccineChildren.Application.DTOs.Response;

public class VaccineScheduleRes
{
    public string ChildrenName { get; set; }
    public string VaccineName { get; set; }
    public DateTime? ScheduleDate { get; set; }
    public string ScheduleStatus { get; set; }
    public string ParentsName { get; set; }
}