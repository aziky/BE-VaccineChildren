
namespace VaccineChildren.Application.DTOs.Request;


public class RegisterRequest
{
    public string UserName { get; set; }
    
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Password { get; set; }
}
