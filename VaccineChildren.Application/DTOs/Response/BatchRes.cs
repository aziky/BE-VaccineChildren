using VaccineChildren.Application.DTOs.Response;
using VaccineChildren.Domain.Entities;

namespace VaccineChildren.Application.DTOs.Responses

{
    public class BatchRes
    {
        public String BatchId { get; set; }
        public Guid? VaccineId { get; set; }
        public DateTime? ProductionDate { get; set; }
        public int? Quantity { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool? IsActive { get; set; }
        public VaccineRes Vaccine { get; set; }
    }
}
