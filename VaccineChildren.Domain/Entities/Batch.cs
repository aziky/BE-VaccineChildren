namespace VaccineChildren.Domain.Entities;

public partial class Batch
{
    public String BatchId { get; set; }

    public Guid? VaccineId { get; set; }

    public DateTime? ProductionDate { get; set; }
    public int? Quantity { get; set; }

    public DateTime? ExpirationDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual VaccineManufacture? Vaccine { get; set; }
}
