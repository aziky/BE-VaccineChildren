using VaccineChildren.Application.DTOs.Response;

namespace VaccineChildren.Application.Services;

public interface IDashboardService
{
    Task<AccountRes> GetAccountAsync(int year);
    Task<RevenueDataRes> GetRevenueDataAsync(int year);
    Task<DashboardSummaryRes> GetDashboardSummaryAsync(int year);
}