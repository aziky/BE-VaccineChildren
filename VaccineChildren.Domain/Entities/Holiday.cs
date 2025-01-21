namespace VaccineChildren.Domain.Entities;

public partial class Holiday
{
    public Guid HolidayId { get; set; } = Guid.NewGuid();

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public Guid? ModifiedBy { get; set; }

    public string? Reason { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual Staff? ModifiedByNavigation { get; set; }
}
