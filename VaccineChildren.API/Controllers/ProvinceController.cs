using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using VaccineChildren.Core.Base;
using VaccineChildren.Domain.Abstraction;
using VaccineChildren.Domain.Models;

namespace VaccineChildren.API.Controllers;

[Route("api")]
[ApiController]
[Authorize (Roles = "user")]
public class ProvinceController : BaseController
{
    private readonly ILogger<ProvinceController> _logger;
    private readonly IProvinceService _provinceService;

    public ProvinceController(ILogger<ProvinceController> logger, IProvinceService provinceService)
    {
        _logger = logger;
        _provinceService = provinceService;
    }

    [HttpGet("province")]
    public async Task<IActionResult> GetProvinceAsync()
    {
        try
        {
            List<ProvinceModel> province = await _provinceService.GetProvincesAsync();
            if (province.IsNullOrEmpty()) throw new Exception("Error getting province");
            return Ok(BaseResponse<List<ProvinceModel>>.OkResponse(province, "Get list provinces successful"));
        }
        catch (Exception e)
        {
            _logger.LogError("Error at getting province async {}", e.Message);
            return HandleException(e, nameof(ProvinceController));
        }
    }
    
    [HttpGet("district/{provinceId}")]
    public async Task<IActionResult> GetDistrictsAsync([FromRoute] string provinceId)
    {
        try
        {
            List<ProvinceModel> province = await _provinceService.GetDistrictsAsync(provinceId);
            if (province.IsNullOrEmpty()) throw new Exception($"Error get districts with province id: {provinceId}");
            return Ok(BaseResponse<List<ProvinceModel>>.OkResponse(province, "Get list distrcit successful"));
        }
        catch (Exception e)
        {
            _logger.LogError("Error at get district async cause by {}", e.Message);
            return HandleException(e, nameof(ProvinceController));
        }
    }
    
    
    [HttpGet("ward/{districtId}")]
    public async Task<IActionResult> GetWardAsync([FromRoute] string districtId)
    {
        try
        {
            List<ProvinceModel> province = await _provinceService.GetWardsAsync(districtId);
            if (province.IsNullOrEmpty()) throw new Exception($"Error get ward with district id: {districtId}");
            return Ok(BaseResponse<List<ProvinceModel>>.OkResponse(province, "Get list ward successful"));
        }
        catch (Exception e)
        {
            _logger.LogError("Error at get ward async cause by {}", e.Message);
            return HandleException(e, nameof(ProvinceController));
        }
    }
}