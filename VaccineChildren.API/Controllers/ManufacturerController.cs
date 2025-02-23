using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Application.DTOs.Response;
using VaccineChildren.Application.Services;
using VaccineChildren.Core.Base;

namespace VaccineChildren.API.Controllers
{
    [Route("api/v1/manufacturer")]
    [ApiController]
    public class ManufacturerController : BaseController
    {
        private readonly ILogger<ManufacturerController> _logger;
        private readonly IManufacturerService _manufacturerService;

        public ManufacturerController(ILogger<ManufacturerController> logger, IManufacturerService manufacturerService)
        {
            _logger = logger;
            _manufacturerService = manufacturerService;
        }

        // POST api/v1/manufacturer
        [HttpPost]
        public async Task<IActionResult> CreateManufacturer([FromBody] ManufacturerReq manufacturerReq)
        {
            try
            {
                _logger.LogInformation("Creating new manufacturer");
                await _manufacturerService.CreateManufacturer(manufacturerReq);
                return Ok(BaseResponse<string>.OkResponse(null, "Manufacturer created successfully"));
            }
            catch (Exception e)
            {
                _logger.LogError("Error while creating manufacturer: {Error}", e.Message);
                return HandleException(e, "Internal Server Error");
            }
        }

        // GET api/v1/manufacturer/{manufacturerId}
        [HttpGet("{manufacturerId:guid}")]
        public async Task<IActionResult> GetManufacturerById(Guid manufacturerId)
        {
            try
            {
                _logger.LogInformation("Fetching manufacturer by ID: {ManufacturerId}", manufacturerId);
                var manufacturer = await _manufacturerService.GetManufacturerById(manufacturerId);
                return Ok(BaseResponse<ManufacturerRes>.OkResponse(manufacturer, "Manufacturer retrieved successfully"));
            }
            catch (KeyNotFoundException e)
            {
                _logger.LogError("Manufacturer not found: {Error}", e.Message);
                return NotFound(BaseResponse<string>.NotFoundResponse("Manufacturer not found"));
            }
            catch (Exception e)
            {
                _logger.LogError("Error while fetching manufacturer: {Error}", e.Message);
                return HandleException(e, "Internal Server Error");
            }
        }

        // PUT api/v1/manufacturer/{manufacturerId}
        [HttpPut("{manufacturerId:guid}")]
        public async Task<IActionResult> UpdateManufacturer(Guid manufacturerId, [FromBody] ManufacturerReq manufacturerReq)
        {
            try
            {
                _logger.LogInformation("Updating manufacturer with ID: {ManufacturerId}", manufacturerId);
                await _manufacturerService.UpdateManufacturer(manufacturerId, manufacturerReq);
                return Ok(BaseResponse<string>.OkResponse(null, "Manufacturer updated successfully"));
            }
            catch (KeyNotFoundException e)
            {
                _logger.LogError("Manufacturer not found: {Error}", e.Message);
                return NotFound(BaseResponse<string>.NotFoundResponse("Manufacturer not found"));
            }
            catch (Exception e)
            {
                _logger.LogError("Error while updating manufacturer: {Error}", e.Message);
                return HandleException(e, "Internal Server Error");
            }
        }

        // DELETE api/v1/manufacturer/{manufacturerId}
        [HttpDelete("{manufacturerId:guid}")]
        public async Task<IActionResult> DeleteManufacturer(Guid manufacturerId)
        {
            try
            {
                _logger.LogInformation("Deleting manufacturer with ID: {ManufacturerId}", manufacturerId);
                await _manufacturerService.DeleteManufacturer(manufacturerId);
                return Ok(BaseResponse<string>.OkResponse(null, "Manufacturer deleted successfully"));
            }
            catch (KeyNotFoundException e)
            {
                _logger.LogError("Manufacturer not found: {Error}", e.Message);
                return NotFound(BaseResponse<string>.NotFoundResponse("Manufacturer not found"));
            }
            catch (Exception e)
            {
                _logger.LogError("Error while deleting manufacturer: {Error}", e.Message);
                return HandleException(e, "Internal Server Error");
            }
        }

        // GET api/v1/manufacturer
        [HttpGet]
        public async Task<IActionResult> GetAllManufacturers()
        {
            try
            {
                _logger.LogInformation("Fetching all manufacturers");
                var manufacturers = await _manufacturerService.GetAllManufacturers();
                return Ok(BaseResponse<List<ManufacturerRes>>.OkResponse(manufacturers, "Manufacturers retrieved successfully"));
            }
            catch (Exception e)
            {
                _logger.LogError("Error while fetching all manufacturers: {Error}", e.Message);
                return HandleException(e, "Internal Server Error");
            }
        }
    }
}
