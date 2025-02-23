using VaccineChildren.Domain.Models;

namespace VaccineChildren.Domain.Abstraction;

public interface IProvinceService
{
    Task<List<ProvinceModel>> GetProvincesAsync();
    Task<List<ProvinceModel>> GetDistrictsAsync(string provinceId);
    Task<List<ProvinceModel>> GetWardsAsync(string districtId);
}