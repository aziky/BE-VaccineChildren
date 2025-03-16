using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using VaccineChildren.Application.Services;
using VaccineChildren.Domain.Models;
using VaccineChildren.Infrastructure.Configuration;
using VaccineChildren.Infrastructure.VNPay;

namespace VaccineChildren.Infrastructure.Implementation;

public class VnPayService : IVnPayService
{
    private readonly VNPayConfig _vnPayConfig;
    private readonly ILogger<VnPayService> _logger;
    private readonly IConfiguration _configuration;
    private readonly string _appUrl;
    
    public VnPayService(VNPayConfig vnPayConfig, ILogger<VnPayService> logger, IConfiguration configuration)
    {
        _vnPayConfig = vnPayConfig;
        _logger = logger;
        _configuration = configuration;
        _appUrl = _configuration.GetValue<string>("AppUrl");
    }
    
    public string CreatePaymentUrl(PaymentInformationModel model, HttpContext context)
    {
        _logger.LogInformation("Start create vn payment url");
        var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_vnPayConfig.TimeZoneId);
        var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
        
        var pay = new VnPayLibrary();

        pay.AddRequestData("vnp_Version", _vnPayConfig.Version);
        pay.AddRequestData("vnp_Command", _vnPayConfig.Command);
        pay.AddRequestData("vnp_TmnCode", _vnPayConfig.TmnCode);
        pay.AddRequestData("vnp_Amount", ((int)model.Amount * 100).ToString());
        pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
        pay.AddRequestData("vnp_ExpireDate", timeNow.AddMinutes(30).ToString("yyyyMMddHHmmss"));
        pay.AddRequestData("vnp_CurrCode", "VND");
        pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
        pay.AddRequestData("vnp_Locale", _vnPayConfig.Locale);
        pay.AddRequestData("vnp_OrderInfo", $"{model.PaymentId}, {model.ChildId}, {model.InjectionDate}");
        pay.AddRequestData("vnp_OrderType", "other");
        pay.AddRequestData("vnp_ReturnUrl", _appUrl + _vnPayConfig.PaymentBackReturnUrl);
        pay.AddRequestData("vnp_TxnRef", model.PaymentId);

        var paymentUrl = pay.CreateRequestUrl(_vnPayConfig.BaseUrl, _vnPayConfig.HashSecret);
        _logger.LogInformation("Done create vn payment url");
        return paymentUrl;

    }

    public PaymentResponseModel PaymentExecute(IQueryCollection collections)
    {
        var pay = new VnPayLibrary();
        var response = pay.GetFullResponseData(collections, _vnPayConfig.HashSecret);
        return response;
    }
    
}