namespace VaccineChildren.Application.DTOs.Response
{
    public class ScheduleRes
    {
        public Guid ChildrenId { get; set; }
        public string VaccineType { get; set; }
        public string ScheduleDate { get; set; }
        public string ScheduleStatus { get; set; }
    }
}