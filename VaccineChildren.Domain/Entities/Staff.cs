using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VaccineChildren.Domain.Entities;

public class Staff
{
    [Key]
    [ForeignKey("User")] // Đánh dấu UserId chính là khóa ngoại trỏ đến User
    public Guid StaffId { get; set; } // StaffId sẽ trùng với UserId

    public DateOnly? Dob { get; set; }
    public string? Gender { get; set; }
    public string? BloodType { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public string Status { get; set; }

    // Navigation Property
    public virtual User User { get; set; } = null!;
    public virtual ICollection<Holiday> Holidays { get; set; } = new List<Holiday>();
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}

