namespace VaccineChildren.Application.DTOs.Requests
{
    public class BatchReq
    {
        public String BatchId { get; set; }
        public Guid? VaccineId { get; set; }
        public DateTime? ProductionDate { get; set; }
        public int? Quantity { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool? IsActive { get; set; }
    }
}
