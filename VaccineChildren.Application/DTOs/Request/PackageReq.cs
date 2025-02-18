namespace VaccineChildren.Application.DTOs.Request
{
    public class PackageReq
    {
        public string PackageName { get; set; }
        public string Description { get; set; }
        public decimal Discount { get; set; }
        public int MinAge { get; set; }
        public int MaxAge { get; set; }
        public string? Unit { get; set; }
        public bool IsActive { get; set; }
        public List<Guid>? VaccineIds { get; set; }

    }
}
