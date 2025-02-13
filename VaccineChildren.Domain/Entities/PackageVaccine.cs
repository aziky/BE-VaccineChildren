namespace VaccineChildren.Domain.Entities;

public partial class PackageVaccine
{

    public Guid VaccineId { get; set; }

    public Guid PackageId { get; set; } 

    public virtual Package Package { get; set; } = null!;
    public virtual Vaccine Vaccine { get; set; } = null!;
}
