using System.Text.Json.Serialization;

namespace VaccineChildren.Domain.Models;

public class ProvinceModel
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("full_name")]
    public string FullName { get; set; }
}