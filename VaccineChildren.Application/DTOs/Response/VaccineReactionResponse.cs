namespace VaccineChildren.Application.DTOs.Response;

public class VaccineReactionResponse
{
    public Guid ReactionId { get; set; }
    public Guid ScheduleId { get; set; }
    public string ChildName { get; set; }
    public Guid VaccineId { get; set; }
    public List<string> Reactions { get; set; }
    public string OtherReactions { get; set; }
    public string Severity { get; set; }
    public DateTime OnsetTime { get; set; }
    public string Notes { get; set; }
}