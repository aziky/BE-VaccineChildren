using System.Text.Json.Serialization;

namespace VaccineChildren.Application.DTOs.Response;

public class StaffRes
{
    public string StaffId { get; set; }
    public string UserName { get; set; }
    public string FullName { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public string Status { get; set; }

    [JsonConverter(typeof(DateOnlyJsonConverter))]
    public DateOnly? Dob { get; set; }

    public string? Gender { get; set; }

    public string? BloodType { get; set; }


}