namespace VaccineChildren.Application.DTOs.Response;

public class VaccineData
{
    public int NumberVaccinated { get; set; }
    public List<VaccineDetails> ListVaccine { get; set; } = new List<VaccineDetails>();

    public class VaccineDetails
    {
        public string VaccineId { get; set; }
        public string VaccineName { get; set; }
    }
}