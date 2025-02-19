namespace VaccineChildren.Application.DTOs.Requests
{
    public class BatchReq
    {
        public Guid? VaccineId { get; set; }
        public DateTime? ProductionDate { get; set; }
        public int? Quantity { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool? IsActive { get; set; }
    }
}
