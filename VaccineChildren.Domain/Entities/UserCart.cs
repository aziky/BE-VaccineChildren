namespace VaccineChildren.Domain.Entities;

public partial class UserCart
{
    public Guid ChildId { get; set; }

    public Guid VaccineId { get; set; }

    public Guid PackageId { get; set; }

    public virtual Child Child { get; set; } = null!;

    public virtual Package Package { get; set; } = null!;

    public virtual Vaccine Vaccine { get; set; } = null!;
}
