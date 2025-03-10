namespace VaccineChildren.Application.DTOs.Response;

public class PaymentHistoryRes
{
    public Guid OrderId { get; set; }
    public string ChildName { get; set; }
    public string PaymentMethod { get; set; }
    public string PaymentDate { get; set; }
    public double Amount { get; set; }
}