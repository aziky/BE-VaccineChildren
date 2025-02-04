namespace VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Domain.Entities;
public class VaccineReq
{
    public string VaccineName { get; set; }
    public string Description { get; set; }
    public int MinAge { get; set; }
    public int MaxAge { get; set; }
    public decimal Price { get; set; }
    public string ManufacturerId { get; set; }

}