using VaccineChildren.Application.DTOs.Response;

namespace VaccineChildren.Application.Services;

public interface IPaymentService
{
    Task<IList<PaymentHistoryRes>> GetPaymentHistory(Guid userId);
    Task<IList<VaccinatedHistory>> GetVaccinatedHistory(Guid payment);
}