using System.ComponentModel.DataAnnotations;

namespace VaccineChildren.Domain.Entities;

public partial class User
{
    [Key]
    public Guid UserId { get; set; } = Guid.NewGuid();

    public int? RoleId { get; set; }

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? FullName { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }
    public bool? IsVerified { get; set; }
    public string? EmailVerificationToken { get; set; }
    public DateTime? TokenExpiry { get; set; }

    public virtual ICollection<Child> Children { get; set; } = new List<Child>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Role? Role { get; set; }

    
    public virtual Staff? Staff { get; set; } 
}
