namespace VaccineChildren.Application.DTOs.Request;

public class VaccineReactionRequest
{
    public Guid ScheduleId { get; set; }
    public List<string> Reactions { get; set; } = new List<string>();
    public string OtherReactions { get; set; }
    public string Severity { get; set; } // "Nhẹ", "Trung bình", "Nặng"
    public string Notes { get; set; }
}