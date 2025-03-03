namespace VaccineChildren.Application.DTOs.Request
{
    public class ScheduleReq
    {
        public List<Guid> VaccineIds { get; set; } = new List<Guid>();
        public Guid ChildId { get; set; }
        public DateTime StartDate { get; set; }
    }
}