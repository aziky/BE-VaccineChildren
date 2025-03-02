namespace VaccineChildren.Application.DTOs.Request
{
    public class ScheduleReq
    {
        public Guid VaccineId { get; set; } // Vẫn dùng VaccineId để tìm vaccine
        public Guid ChildId { get; set; }
        public DateTime StartDate { get; set; }
    }
}