namespace VaccineChildren.Application.DTOs.Request;


public class ManufacturerRes
{
    public Guid ManufacturerId { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = null!;

    public string? ShortName { get; set; }

    public string? Description { get; set; }

    public string? CountryName { get; set; }

    public string? CountryCode { get; set; }

    public bool? IsActive { get; set; }


}