namespace VaccineChildren.Application.DTOs.Response;

using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Domain.Entities;

public class DescriptionDetail
{
    public string Info { get; set; }
    public string TargetedPatient { get; set; }
    public string InjectionSchedule { get; set; }
    public string VaccineReaction { get; set; }
}

public class VaccineRes
{
    public string VaccineId { get; set; }
    public string? VaccineName { get; set; }
    public DescriptionDetail? Description { get; set; }
    public int? MinAge { get; set; }
    public int? MaxAge { get; set; }
    public int? NumberDose { get; set; }
    public int? Duration { get; set; }
    public string? Unit { get; set; }
    public string? Image { get; set; }
    public bool? IsActive { get; set; }
    public List<ManufacturerRes> Manufacturers { get; set; } = new List<ManufacturerRes>();
    public decimal Price { get; set; }
}