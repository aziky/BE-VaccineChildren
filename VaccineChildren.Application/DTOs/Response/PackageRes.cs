namespace VaccineChildren.Application.DTOs.Response
{
    public class PackageRes
    {
        public Guid PackageId { get; set; }
        public string? PackageName { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public decimal? Discount { get; set; }
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }
        public string? Unit { get; set; }
        public bool? IsActive { get; set; }
        public List<VaccineRes>? Vaccines { get; set; }

    }
}
