namespace VaccineChildren.Application.DTOs.Response;

public class VaccineScheduleRes
{
    public Guid ScheduleId { get; set; }
    public string ChildrenName { get; set; }
    public string VaccineName { get; set; }
    public string ScheduleDate { get; set; } = string.Empty;

    public string ScheduleStatus { get; set; }
    public string ParentsName { get; set; }
    public string PhoneNumber { get; set; }
}