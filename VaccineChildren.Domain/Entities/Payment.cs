namespace VaccineChildren.Domain.Entities;

public partial class Payment
{
    public Guid PaymentId { get; set; } = Guid.NewGuid();

    public Guid? OrderId { get; set; }

    public Guid? UserId { get; set; }

    public decimal? Amount { get; set; }

    public string? PaymentMethod { get; set; }

    public string? PaymentStatus { get; set; }

    public DateTime? PaymentDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual Order? Order { get; set; }

    public virtual User? User { get; set; }
}
