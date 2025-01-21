namespace VaccineChildren.Domain.Entities;

public partial class Child
{
    public Guid ChildId { get; set; } = Guid.NewGuid();

    public Guid? UserId { get; set; }

    public string? FullName { get; set; }

    public string? Notes { get; set; }

    public DateOnly? Dob { get; set; }

    public string? Gender { get; set; }

    public string? BloodType { get; set; }

    public string? AllergiesNotes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    public virtual User? User { get; set; }

    public virtual ICollection<UserCart> UserCarts { get; set; } = new List<UserCart>();
}
