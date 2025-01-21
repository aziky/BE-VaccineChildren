namespace VaccineChildren.Domain.Entities;

public partial class Notification
{
    public Guid NotificationId { get; set; } = Guid.NewGuid();

    public Guid? UserId { get; set; }

    public int? TemplateId { get; set; }

    public Guid? ChildId { get; set; }

    public bool? IsRead { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual Child? Child { get; set; }

    public virtual Template? Template { get; set; }

    public virtual User? User { get; set; }
}
