using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VaccineChildren.Domain.Entities;

public partial class Template
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TemplateId { get; set; }

    public string? Type { get; set; }

    public string? Subject { get; set; }

    public string? Temaplate { get; set; }

    public bool? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
