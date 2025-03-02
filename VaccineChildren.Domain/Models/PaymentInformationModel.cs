namespace VaccineChildren.Domain.Models;

public class PaymentInformationModel
{
    public string OrderType { get; set; }
    public double Amount { get; set; }
    public string OrderDescription { get; set; }
    public string Name { get; set; }
    
    public string PaymentId { get; set; }

}