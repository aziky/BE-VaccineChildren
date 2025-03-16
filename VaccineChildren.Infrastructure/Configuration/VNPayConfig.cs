namespace VaccineChildren.Infrastructure.Configuration;

public class VNPayConfig
{
    public string TmnCode { get; set; }
    public string HashSecret { get; set; }
    public string BaseUrl { get; set; }
    public string Command { get; set; }
    public string Version { get; set; }
    public string Locale { get; set; }
    public string PaymentBackReturnUrl { get; set; }
    public string TimeZoneId { get; set; }
    public string PaymentSuccessUrl { get; set; }
    public string PaymentCancelUrl { get; set; }
}