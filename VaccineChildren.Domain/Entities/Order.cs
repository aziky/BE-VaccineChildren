namespace VaccineChildren.Domain.Entities;

public partial class Order
{
    public Guid OrderId { get; set; } = Guid.NewGuid();

    public Guid? ChildId { get; set; }

    public Guid? PackageId { get; set; }

    public Guid? ApprovedStaff { get; set; }

    public DateTime? OrderDate { get; set; }

    public string? Status { get; set; }

    public bool? PackageModified { get; set; }

    public string? Notes { get; set; }

    public bool? IsConfirmed { get; set; }

    public DateTime? ConfirmedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual Staff? ApprovedStaffNavigation { get; set; }

    public virtual Child? Child { get; set; }

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual Package? Package { get; set; }

    public virtual Payment? Payment { get; set; }

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    public virtual ICollection<Package> Packages { get; set; } = new List<Package>();

    public virtual ICollection<VaccineManufacture> Vaccines { get; set; } = new List<VaccineManufacture>();
}
