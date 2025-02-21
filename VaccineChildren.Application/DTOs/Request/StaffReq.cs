using System.Text.Json.Serialization;

namespace VaccineChildren.Application.DTOs.Request;


public class StaffReq
{
    public string Username { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Password { get; set; }  
    public string Email { get; set; }
    public string FullName { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    [JsonConverter(typeof(DateOnlyJsonConverter))]
    public DateOnly? Dob { get; set; }

    public string? Gender { get; set; }

    public string? BloodType { get; set; }

    public string? Status { get; set; } 
}