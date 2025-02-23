using System.Text.Json;
using Microsoft.Extensions.Logging;
using VaccineChildren.Domain.Abstraction;
using VaccineChildren.Domain.Models;

namespace VaccineChildren.Infrastructure.Implementation;

public class ProvinceService : IProvinceService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProvinceService> _logger;
    private const string ProvinceUrl = "https://esgoo.net/api-tinhthanh/1/0.htm";
    private const string DistrictUrl = "https://esgoo.net/api-tinhthanh/2/{provinceId}.htm";
    private const string WardUrl = "https://esgoo.net/api-tinhthanh/3/{districtId}.htm";

    public ProvinceService(HttpClient httpClient, ILogger<ProvinceService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    private async Task<List<T>> FetchDataAsync<T>(string url)
    {
        try
        {
            _logger.LogInformation($"Fetching data from: {url}");
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error fetching data: {response.StatusCode}");
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ProvinceResponse<T>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return result?.Data ?? new List<T>();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching data: {ex.Message}");
            return new List<T>();
        }
    }

    public async Task<List<ProvinceModel>> GetProvincesAsync()
    {
        return await FetchDataAsync<ProvinceModel>(ProvinceUrl);
    }

    public async Task<List<ProvinceModel>> GetDistrictsAsync(string provinceId)
    {
        return await FetchDataAsync<ProvinceModel>(DistrictUrl.Replace("{provinceId}", provinceId));
    }
    
    public async Task<List<ProvinceModel>> GetWardsAsync(string districtId)
    {
        return await FetchDataAsync<ProvinceModel>(WardUrl.Replace("{districtId}", districtId));
    }
}