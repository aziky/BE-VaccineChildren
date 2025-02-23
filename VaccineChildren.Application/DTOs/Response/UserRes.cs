namespace VaccineChildren.Application.DTOs.Response;

public class UserRes
{
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string? Token { get; set; }
    public string RoleName { get; set; }
}