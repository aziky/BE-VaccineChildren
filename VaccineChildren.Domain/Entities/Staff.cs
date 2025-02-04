namespace VaccineChildren.Domain.Entities;

public partial class Staff
{
    public Guid StaffId { get; set; } = Guid.NewGuid();

    public DateOnly? Dob { get; set; }

    public string? Gender { get; set; }

    public string? BloodType { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public string Status { get; set; }

    public Guid UserId { get; set; }

    public int RoleId { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<Holiday> Holidays { get; set; } = new List<Holiday>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

}
