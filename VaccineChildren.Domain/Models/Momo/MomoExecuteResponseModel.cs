namespace VaccineChildren.Domain.Models.Momo;

public class MomoExecuteResponseModel
{
    public string OrderId { get; set; }
    public string Amount { get; set; }
    public string FullName { get; set; }
    public string OrderInfo { get; set; }
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
    public string ErrorCode { get; set; }
    
    public string PaymentMethod { get; set; }

}