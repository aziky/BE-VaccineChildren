using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using VaccineChildren.Application.Services;
using VaccineChildren.Domain.Models;
using VaccineChildren.Domain.Models.Momo;
using VaccineChildren.Infrastructure.Configuration;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using VaccineChildren.Core.Store;

namespace VaccineChildren.Infrastructure.Implementation;

public class MomoService : IMomoService
{
    private readonly ILogger<MomoService> _logger;
    private readonly MomoConfig _momoConfig;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly string _appUrl;


    public MomoService(MomoConfig momoConfig, HttpClient httpClient, ILogger<MomoService> logger, IConfiguration configuration)
    {
        _momoConfig = momoConfig;
        _httpClient = httpClient;
        _logger = logger;
        _configuration = configuration;
        _appUrl = _configuration.GetValue<string>("AppUrl");

    }

    public async Task<MomoCreatePaymentResponseModel?> CreatePaymentAsync(PaymentInformationModel model)
    {
        try
        {
            var rawData =
                $"partnerCode={_momoConfig.PartnerCode}" +
                $"&accessKey={_momoConfig.AccessKey}" +
                $"&requestId={model.PaymentId}" +
                $"&amount={model.Amount}" +
                $"&orderId={model.PaymentId}" +
                $"&orderInfo={model.OrderInfo}" +
                $"&returnUrl={_appUrl + _momoConfig.ReturnUrl}" +
                $"&notifyUrl={_appUrl + _momoConfig.NotifyUrl}" +
                $"&extraData=";

            var signature = ComputeHmacSha256(rawData, _momoConfig.SecretKey);
            var requestData = new
            {
                accessKey = _momoConfig.AccessKey,
                partnerCode = _momoConfig.PartnerCode,
                requestType = _momoConfig.RequestType,
                notifyUrl = _appUrl + _momoConfig.NotifyUrl,
                returnUrl = _appUrl + _momoConfig.ReturnUrl,
                orderId = model.PaymentId,
                amount = model.Amount.ToString(),
                orderInfo = model.OrderInfo,
                requestId = model.PaymentId,
                requestTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + (30 * 60 * 1000),
                extraData = "",
                signature = signature
            };

            var json = JsonSerializer.Serialize(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));
            var response = await _httpClient.PostAsync(_momoConfig.MomoApiUrl, content, cts.Token);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}");
            }
            

            var responseBody = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Response from MoMo: {}", responseBody);

            return JsonSerializer.Deserialize<MomoCreatePaymentResponseModel>(responseBody);
        }
        catch (Exception e)
        {
            _logger.LogError("Error when creating momo payment cause by {}", e.Message);
            throw;
        }
    }

    public MomoExecuteResponseModel GetFullResponseData(IQueryCollection query)
    {
        var response = MappingFromQuery(query);

        var rawData = new
        {
            partnerCode = response.PartnerCode,
            accessKey = response.AccessKey,
            requestId = response.RequestId,
            amount = response.Amount,
            orderId = response.OrderId,
            orderInfo = response.OrderInfo,
            orderType = response.OrderType,
            transId = response.TransId,
            message = response.Message,
            localMessage = response.LocalMessage,
            responseTime = response.ResponseTime,
            errorCode = response.ErrorCode,
            payType = response.PayType,
            extraData = response.ExtraData,
        };
        
        string queryString = string.Join("&", rawData.GetType().GetProperties()
            .Select(prop => $"{prop.Name}={prop.GetValue(rawData)}"));
        var computedSignature = ComputeHmacSha256(queryString, _momoConfig.SecretKey);
        bool isValid = computedSignature.Equals(response.Signature, StringComparison.OrdinalIgnoreCase);
        
        if (!isValid)
        {
            return new MomoExecuteResponseModel()
            {
                Success = false,
                ErrorCode = response.ErrorCode,
                ErrorMessage = response.LocalMessage
            };
        }
        
        return new MomoExecuteResponseModel()
        {
            Success = true,
            Amount = response.Amount,
            OrderId = response.OrderId,
            OrderInfo = response.OrderInfo,
            ErrorCode = response.ErrorCode,
            ErrorMessage = response.LocalMessage,
            PaymentMethod = StaticEnum.PaymentMethodEnum.Momo.Name(),
        };
    }

    private MomoPaymentResponse MappingFromQuery(IQueryCollection query)
    {
        return new MomoPaymentResponse()
        {
            PartnerCode = query["PartnerCode"],
            AccessKey = query["AccessKey"],
            RequestId = query["RequestId"],
            Amount = query["Amount"],
            OrderId = query["OrderId"],
            OrderInfo = query["OrderInfo"],
            OrderType = query["OrderType"],
            TransId = query["TransId"],
            Message = query["Message"],
            LocalMessage = query["LocalMessage"],
            ResponseTime = query["ResponseTime"],
            ErrorCode = query["ErrorCode"],
            PayType = query["PayType"],
            ExtraData = query["ExtraData"],
            Signature = query["Signature"]
        };
    }
    
    
    private string BuildSignatureString(IQueryCollection collection)
    {
        var filteredParams = collection.Where(kvp => kvp.Key != "Signature")
            .OrderBy(kvp => kvp.Key) 
            .Select(kvp => $"{kvp.Key}={kvp.Value}");

        return string.Join("&", filteredParams);
    }
    


    private string ComputeHmacSha256(string message, string secretKey)
    {
        var keyBytes = Encoding.UTF8.GetBytes(secretKey);
        var messageBytes = Encoding.UTF8.GetBytes(message);

        byte[] hashBytes;

        using (var hmac = new HMACSHA256(keyBytes))
        {
            hashBytes = hmac.ComputeHash(messageBytes);
        }

        var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

        return hashString;
    }
}