namespace VaccineChildren.Application.DTOs.Request;

public class CreateChildReq
{
    public string UserId { get; set; }
    public string FullName { get; set; }
    public string Dob { get; set; }
    public string Gender { get; set; }
    public string Address { get; set; }
}