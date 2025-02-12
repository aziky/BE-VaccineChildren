namespace VaccineChildren.Application.DTOs.Request;

public class CreateAppointmentReq
{
    
    public string UserId { get; set; }
    public string FullName { get; set; }
    public string Dob { get; set; }
    public string Gender { get; set; }
    public string Address { get; set; }
    public string InjectionDate { get; set; }
    public double Amount { get; set; }
    public IList<string> PackageIdList { get; set; }
    public IList<string> VaccineIdList { get; set; }
    
}