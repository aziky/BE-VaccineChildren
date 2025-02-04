namespace VaccineChildren.Application.DTOs.Response;
using VaccineChildren.Domain.Entities;

public class VaccineRes
{
    public string VaccineName { get; set; }
    public string Description { get; set; }
    public int MinAge { get; set; }
    public int MaxAge { get; set; }
    public decimal Price { get; set; }
    public string ManufacturerName { get; set; }
    public Boolean IsActive { get; set; }

}