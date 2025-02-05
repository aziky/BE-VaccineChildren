namespace VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Domain.Entities;

public class StaffReq
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public int RoleId { get; set; }
    public DateOnly? Dob { get; set; }

    public string? Gender { get; set; }

    public string? BloodType { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

}