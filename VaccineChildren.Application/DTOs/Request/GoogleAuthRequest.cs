using System.Text.Json.Serialization;

namespace VaccineChildren.Application.DTOs.Request;

public class GoogleAuthRequest
{
    public string IdToken { get; set; }
    public string Username { get; set; }
    // public string AccessToken { get; set; }
}
public class GoogleUserInfo
{
    [JsonPropertyName("sub")]
    public string Id { get; set; }
    [JsonPropertyName("email")]
    public string Email { get; set; }
    
    public bool VerifiedEmail { get; set; }
    public string Name { get; set; }
    public string GivenName { get; set; }
    public string FamilyName { get; set; }
    public string Picture { get; set; }
    public string Locale { get; set; }
}