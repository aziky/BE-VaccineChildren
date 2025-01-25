using VaccineChildren.Application.DTOs.Response;

namespace VaccineChildren.Application.Services;

public interface IDashboardService
{
    Task<AccountRes> GetAccountAsync();
    
}