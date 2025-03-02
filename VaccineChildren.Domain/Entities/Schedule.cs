namespace VaccineChildren.Domain.Entities;

public partial class Schedule
{
    public Guid ScheduleId { get; set; } = Guid.NewGuid();

    public Guid? OrderId { get; set; }

    public Guid? ChildId { get; set; }

    public string? VaccineType { get; set; }

    public Guid? AdministeredBy { get; set; }

    public DateTime? ScheduleDate { get; set; }

    public bool? IsVaccinated { get; set; }

    public DateTime? ActualDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual Staff? AdministeredByNavigation { get; set; }

    public virtual Child? Child { get; set; }

    public virtual Order? Order { get; set; }
    public string? PreVaccineCheckup { get; set; }
    public string? status { get; set; }

    public virtual ICollection<VaccineReaction> VaccineReactions { get; set; } = new List<VaccineReaction>();
}
