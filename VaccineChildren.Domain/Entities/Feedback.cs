namespace VaccineChildren.Domain.Entities;

public partial class Feedback
{
    public Guid FeedbackId { get; set; } = Guid.NewGuid();

    public Guid? OrderId { get; set; }

    public Guid? UserId { get; set; }

    public int? Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual Order? Order { get; set; }

    public virtual User? User { get; set; }
}
