namespace VaccineChildren.Domain.Entities;

public partial class Vaccine
{
    public Guid VaccineId { get; set; } = Guid.NewGuid();

    public string? VaccineName { get; set; }

    public string? Description { get; set; }

    public int? MinAge { get; set; }

    public int? MaxAge { get; set; }

    public int? NumberDose { get; set; }
    public string? Image { get; set; }
    public int? Duration { get; set; }

    public string? Unit { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual ICollection<UserCart> UserCarts { get; set; } = new List<UserCart>();

    public virtual ICollection<VaccineManufacture> VaccineManufactures { get; set; } = new List<VaccineManufacture>();
    public virtual ICollection<Package> Packages { get; set; } = new List<Package>();
    
}
