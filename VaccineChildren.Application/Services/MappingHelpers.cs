using VaccineChildren.Application.DTOs.Response;

namespace VaccineChildren.Application;

public static class MappingHelpers
{
    public static string SerializeDescription(DescriptionDetail description)
    {
        return System.Text.Json.JsonSerializer.Serialize(description ?? new DescriptionDetail
        {
            Info = "N/A",
            TargetedPatient = "N/A",
            VaccineReaction = "N/A"
        });
    }

    public static DescriptionDetail DeserializeDescription(string jsonDescription)
    {
        return System.Text.Json.JsonSerializer.Deserialize<DescriptionDetail>(jsonDescription);
    }
}