namespace VaccineChildren.Application.DTOs.Response;

public class VaccinatedHistory
{
    public string ChildName { get; set; }
    public string VaccineName { get; set; }
    public IList<VaccinatedDate> VaccinatedDates { get; set; } = new List<VaccinatedDate>();
    
  
}