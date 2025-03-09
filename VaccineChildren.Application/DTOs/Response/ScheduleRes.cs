namespace VaccineChildren.Application.DTOs.Response
{
    public class ScheduleRes
    {
        public Guid ChildId { get; set; }
        public Guid VaccineId { get; set; }
        public string ScheduleDate { get; set; }
    }
}