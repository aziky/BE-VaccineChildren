namespace VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Domain.Entities;
public class VaccineReq
{
    public string? VaccineName { get; set; }

    public string? Description { get; set; }

    public int? MinAge { get; set; }

    public int? MaxAge { get; set; }

    public int? NumberDose { get; set; }

    public int? Duration { get; set; }

    public string? Unit { get; set; }

    public bool? IsActive { get; set; }
    public string? Image { get; set; }
    
    public string ManufacturerId { get; set; }
    public decimal Price { get; set; }
    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

}