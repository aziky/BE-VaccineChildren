namespace VaccineChildren.Application.DTOs.Request;

public class CreateOrderReq
{
    
    public Guid UserId { get; set; }
    public Guid ChildId { get; set; }
    public string InjectionDate { get; set; }
    public double Amount { get; set; }
    public List<PackageModified> PackageList { get; set; } = new();
    public List<string> VaccineIdList { get; set; } = new();
    
    public class PackageModified
    {
        public string PackageId { get; set; }
        public bool Modified { get; set; }
        public List<string> VaccineModifiedIdList { get; set; } = new();
    }
    
    public string PaymentMethod { get; set; }

}