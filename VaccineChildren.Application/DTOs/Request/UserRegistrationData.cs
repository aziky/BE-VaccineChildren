using VaccineChildren.Domain.Entities;

namespace VaccineChildren.Application.DTOs.Request;

public class UserRegistrationData
{
    public string VerificationToken { get; set; }
    public User UserData { get; set; }
}