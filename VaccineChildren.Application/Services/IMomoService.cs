using Microsoft.AspNetCore.Http;
using VaccineChildren.Domain.Models;
using VaccineChildren.Domain.Models.Momo;

namespace VaccineChildren.Application.Services;

public interface IMomoService
{
    Task<MomoCreatePaymentResponseModel?> CreatePaymentAsync(PaymentInformationModel model);
    MomoExecuteResponseModel GetFullResponseData(IQueryCollection collection);
}