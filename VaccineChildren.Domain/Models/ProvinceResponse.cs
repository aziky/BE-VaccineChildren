using System.Text.Json.Serialization;

namespace VaccineChildren.Domain.Models;

public class ProvinceResponse<T>
{
    [JsonPropertyName("error")]
    public int Error { get; set; }
    
    [JsonPropertyName("error_text")]
    public string ErrorText { get; set; }

    [JsonPropertyName("data_name")]
    public string? DataName { get; set; }

    [JsonPropertyName("data")]
    public List<T> Data { get; set; }
}