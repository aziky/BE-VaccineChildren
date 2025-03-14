namespace VaccineChildren.Application.DTOs.Request;

public class ResetPasswordRequest
{
    public string Email { get; set; }
    public string Token { get; set; }
    public string NewPassword { get; set; }
}
public class PasswordResetData
{
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
}