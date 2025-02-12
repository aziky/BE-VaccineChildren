namespace VaccineChildren.Application.DTOs.Request
{
    public class PackageReq
    {
        public string? PackageName { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public decimal? Discount { get; set; }
        public bool? IsActive { get; set; }
    }
}
