using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Application.DTOs.Response;
using VaccineChildren.Application.Services;
using VaccineChildren.Core.Base;

namespace VaccineChildren.API.Controllers;

[Route("api/v1/staff")]  
[ApiController]
[Authorize (Roles = "admin")]
public class StaffController : BaseController
{
    private readonly ILogger<StaffController> _logger;
    private readonly IStaffService _staffService;

    public StaffController(ILogger<StaffController> logger, IStaffService staffService)
    {
        _logger = logger;
        _staffService = staffService;
    }

    // POST api/v1/staff
    [HttpPost]
    public async Task<IActionResult> CreateStaff([FromBody] StaffReq staffReq)
    {
        try
        {
            _logger.LogInformation("Creating new staff");
            await _staffService.CreateStaff(staffReq);
            return Ok(BaseResponse<string>.OkResponse(null, "Staff created successfully"));
        }
        catch (Exception e)
        {
            _logger.LogError("Error while creating staff: {Error}", e.Message);
            return HandleException(e, "Internal Server Error");
        }
    }

    // GET api/v1/staff/{staffId}
    [HttpGet("{staffId:guid}")]
    public async Task<IActionResult> GetStaffById(Guid staffId)
    {
        try
        {
            _logger.LogInformation("Fetching staff by ID: {StaffId}", staffId);
            var staff = await _staffService.GetStaffById(staffId);
            return Ok(BaseResponse<StaffRes>.OkResponse(staff, "Staff retrieved successfully"));
        }
        catch (KeyNotFoundException e)
        {
            _logger.LogError("Staff not found: {Error}", e.Message);
            return NotFound(BaseResponse<string>.NotFoundResponse("Staff not found"));
        }
        catch (Exception e)
        {
            _logger.LogError("Error while fetching staff: {Error}", e.Message);
            return HandleException(e, "Internal Server Error");
        }
    }

    // PUT api/v1/staff/{staffId}
    [HttpPut("{staffId:guid}")]
    public async Task<IActionResult> UpdateStaff(Guid staffId, [FromBody] StaffReq staffReq)
    {
        try
        {
            _logger.LogInformation("Updating staff with ID: {StaffId}", staffId);
            await _staffService.UpdateStaff(staffId, staffReq);
            return Ok(BaseResponse<string>.OkResponse(null, "Staff updated successfully"));
        }
        catch (KeyNotFoundException e)
        {
            _logger.LogError("Staff not found: {Error}", e.Message);
            return NotFound(BaseResponse<string>.NotFoundResponse("Staff not found"));
        }
        catch (Exception e)
        {
            _logger.LogError("Error while updating staff: {Error}", e.Message);
            return HandleException(e, "Internal Server Error");
        }
    }

    // DELETE api/v1/staff/{staffId}
    [HttpDelete("{staffId:guid}")]
    public async Task<IActionResult> DeleteStaff(Guid staffId)
    {
        try
        {
            _logger.LogInformation("Deleting staff with ID: {StaffId}", staffId);
            await _staffService.DeleteStaff(staffId);
            return Ok(BaseResponse<string>.OkResponse(null, "Staff deleted successfully"));
        }
        catch (KeyNotFoundException e)
        {
            _logger.LogError("Staff not found: {Error}", e.Message);
            return NotFound(BaseResponse<string>.NotFoundResponse("Staff not found"));
        }
        catch (Exception e)
        {
            _logger.LogError("Error while deleting staff: {Error}", e.Message);
            return HandleException(e, "Internal Server Error");
        }
    }

    // GET api/v1/staff
    [HttpGet]
    public async Task<IActionResult> GetAllStaff()
    {
        try
        {
            _logger.LogInformation("Fetching all staff");
            var staffList = await _staffService.GetAllStaff();
            return Ok(BaseResponse<List<StaffRes>>.OkResponse(staffList, "Staff retrieved successfully"));
        }
        catch (Exception e)
        {
            _logger.LogError("Error while fetching all staff: {Error}", e.Message);
            return HandleException(e, "Internal Server Error");
        }
    }
}