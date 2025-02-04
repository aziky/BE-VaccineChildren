namespace VaccineChildren.Application.DTOs.Response;

public class DashboardSummaryRes
{
    public int Year { get; set; } 
    public int TotalAvailableVaccines { get; set; }
    public int TotalVaccinatedCustomers { get; set; }
    public int TotalAvailablePackages { get; set; }
    public List<AgeData> AgeData { get; set; }
    public List<ManufacturerData> ManufacturerData { get; set; }
    public List<VaccineData> VaccineData { get; set; }
}