namespace VaccineChildren.Application.DTOs.Response;
using VaccineChildren.Domain.Entities;

public class VaccineRes
{
    public string VaccineId {get; set;}
    public string? VaccineName { get; set; }

    public string? Description { get; set; }

    public int? MinAge { get; set; }

    public int? MaxAge { get; set; }

    public int? NumberDose { get; set; }

    public int? Duration { get; set; }

    public string? Unit { get; set; }
    public string? Image { get; set; }

    public bool? IsActive { get; set; }

    public string ManufacturerName { get; set; }
    public decimal Price { get; set; }
    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }
}