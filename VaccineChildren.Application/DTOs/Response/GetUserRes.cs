namespace VaccineChildren.Application.DTOs.Response;

public class GetUserRes
{
    public string UserId { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public IList<GetChildRes> ListChildRes { get; set; }
}