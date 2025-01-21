namespace VaccineChildren.Domain.Entities;

public partial class VaccineManufacture
{
    public Guid ManufacturerId { get; set; }

    public Guid VaccineId { get; set; }

    public decimal? Price { get; set; }

    public virtual ICollection<Batch> Batches { get; set; } = new List<Batch>();

    public virtual Manufacturer Manufacturer { get; set; } = null!;

    public virtual Vaccine Vaccine { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
