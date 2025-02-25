using Microsoft.AspNetCore.Http;
using VaccineChildren.Domain.Models;

namespace VaccineChildren.Domain.Abstraction;

public interface IVnPayService
{
    string CreatePaymentUrl(PaymentInformationModel model, HttpContext context);
    PaymentResponseModel PaymentExecute(IQueryCollection collections);

}