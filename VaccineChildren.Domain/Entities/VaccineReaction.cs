namespace VaccineChildren.Domain.Entities;

public partial class VaccineReaction
{
    public Guid ReactionId { get; set; } = Guid.NewGuid();

    public Guid? ScheduleId { get; set; }

    public string? ReactionDescription { get; set; }

    public string? Severity { get; set; }

    public DateTime? OnsetTime { get; set; }

    public DateTime? ResolvedTime { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual Schedule? Schedule { get; set; }
}
