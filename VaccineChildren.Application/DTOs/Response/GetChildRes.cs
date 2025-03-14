namespace VaccineChildren.Application.DTOs.Response;

public class GetChildRes
{
    public string OrderId { get; set; }
    public string ChildId { get; set; }
    public string FullName { get; set; }
    public string Dob { get; set; }
    public string Gender { get; set; }
    public string Address { get; set; }
    public IList<VaccinatedInfor> VaccinatedInformation { get; set; } = new List<VaccinatedInfor>();
    
    public class VaccinatedInfor
    {
        public string ScheduleId { get; set; }
        public string ManufacturerName { get; set; }
        public string VaccineName { get; set; }
        public DateTime? ScheduleDate { get; set; }
        public DateTime? ActualDate { get; set; }
        public bool IsVaccinated { get; set; }
    }
    
}