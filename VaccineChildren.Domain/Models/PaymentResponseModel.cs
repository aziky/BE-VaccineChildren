namespace VaccineChildren.Domain.Models;

public class PaymentResponseModel
{
    public string OrderInfo { get; set; }
    public string TransactionId { get; set; }
    public string OrderId { get; set; }
    public string PaymentMethod { get; set; }
    public bool Success { get; set; }
    public string Token { get; set; }
    public string VnPayResponseCode { get; set; }
    public string VnTransactionStatus { get; set; }

    public override string ToString()
    {
        return $"PaymentResponseModel: {{ " +
               $"OrderInfo: \"{OrderInfo}\", " +
               $"TransactionId: \"{TransactionId}\", " +
               $"OrderId: \"{OrderId}\", " +
               $"PaymentMethod: \"{PaymentMethod}\", " +
               $"Success: {Success}, " +
               $"Token: \"{Token}\", " +
               $"VnPayResponseCode: \"{VnPayResponseCode}\", " +
               $"VnTransactionStatus: \"{VnTransactionStatus}\" " +
               $"}}";
    }
}