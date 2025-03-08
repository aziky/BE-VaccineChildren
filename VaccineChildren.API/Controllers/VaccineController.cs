using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Application.DTOs.Response;
using VaccineChildren.Application.Services;
using VaccineChildren.Core.Base;
using VaccineChildren.Domain.Entities;

namespace VaccineChildren.API.Controllers
{
    [Route("api/v1/vaccine")]
    [ApiController]
    public class VaccineController : BaseController
    {
        private readonly ILogger<VaccineController> _logger;
        private readonly IVaccineService _vaccineService;

        public VaccineController(ILogger<VaccineController> logger, IVaccineService vaccineService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _vaccineService = vaccineService ?? throw new ArgumentNullException(nameof(vaccineService));
        }

        // POST api/v1/vaccine
        [HttpPost]
        public async Task<IActionResult> CreateVaccine([FromBody] VaccineReq vaccineReq)
        {
            try
            {
                if (vaccineReq == null)
                {
                    return BadRequest(BaseResponse<string>.BadRequestResponse("Vaccine request cannot be null"));
                }

                _logger.LogInformation("Creating new vaccine");
                await _vaccineService.CreateVaccine(vaccineReq);
                return Ok(BaseResponse<string>.OkResponse(null, "Vaccine created successfully"));
            }
            catch (ValidationException e)
            {
                _logger.LogWarning("Validation error while creating vaccine: {Error}", e.Message);
                return BadRequest(BaseResponse<string>.BadRequestResponse(e.Message));
            }
            catch (Exception e)
            {
                _logger.LogError("Error while creating vaccine: {Error}", e.Message);
                return HandleException(e, "Internal Server Error");
            }
        }

        // GET api/v1/vaccine/{vaccineId}
        [HttpGet("{vaccineId:guid}")]
        public async Task<IActionResult> GetVaccineById(Guid vaccineId)
        {
            try
            {
                _logger.LogInformation("Fetching vaccine by ID: {VaccineId}", vaccineId);
                var vaccine = await _vaccineService.GetVaccineById(vaccineId);
                return Ok(BaseResponse<VaccineRes>.OkResponse(vaccine, "Vaccine retrieved successfully"));
            }
            catch (KeyNotFoundException e)
            {
                _logger.LogWarning("Vaccine not found: {Error}", e.Message);
                return NotFound(BaseResponse<string>.NotFoundResponse(e.Message));
            }
            catch (Exception e)
            {
                _logger.LogError("Error while fetching vaccine: {Error}", e.Message);
                return HandleException(e, "Internal Server Error");
            }
        }

        // PUT api/v1/vaccine/{vaccineId}
        [HttpPut("{vaccineId:guid}")]
        public async Task<IActionResult> UpdateVaccine(Guid vaccineId, [FromBody] VaccineReq vaccineReq)
        {
            try
            {
                if (vaccineReq == null)
                {
                    return BadRequest(BaseResponse<string>.BadRequestResponse("Vaccine request cannot be null"));
                }

                _logger.LogInformation("Updating vaccine with ID: {VaccineId}", vaccineId);
                await _vaccineService.UpdateVaccine(vaccineId, vaccineReq);
                return Ok(BaseResponse<string>.OkResponse(null, "Vaccine updated successfully"));
            }
            catch (KeyNotFoundException e)
            {
                _logger.LogWarning("Vaccine not found: {Error}", e.Message);
                return NotFound(BaseResponse<string>.NotFoundResponse(e.Message));
            }
            catch (ValidationException e)
            {
                _logger.LogWarning("Validation error while updating vaccine: {Error}", e.Message);
                return BadRequest(BaseResponse<string>.BadRequestResponse(e.Message));
            }
            catch (Exception e)
            {
                _logger.LogError("Error while updating vaccine: {Error}", e.Message);
                return HandleException(e, "Internal Server Error");
            }
        }

        // DELETE api/v1/vaccine/{vaccineId}
        [HttpDelete("{vaccineId:guid}")]
        public async Task<IActionResult> DeleteVaccine(Guid vaccineId)
        {
            try
            {
                _logger.LogInformation("Deleting vaccine with ID: {VaccineId}", vaccineId);
                await _vaccineService.DeleteVaccine(vaccineId);
                return Ok(BaseResponse<string>.OkResponse(null, "Vaccine deleted successfully"));
            }
            catch (KeyNotFoundException e)
            {
                _logger.LogWarning("Vaccine not found: {Error}", e.Message);
                return NotFound(BaseResponse<string>.NotFoundResponse(e.Message));
            }
            catch (Exception e)
            {
                _logger.LogError("Error while deleting vaccine: {Error}", e.Message);
                return HandleException(e, "Internal Server Error");
            }
        }

        // GET api/v1/vaccine
        [HttpGet]
        public async Task<IActionResult> GetAllVaccines([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                if (pageIndex < 1 || pageSize < 1)
                {
                    return BadRequest(BaseResponse<string>.BadRequestResponse("PageIndex and PageSize must be greater than 0"));
                }

                _logger.LogInformation("Fetching all vaccines, page {PageIndex}, size {PageSize}", pageIndex, pageSize);
                var (vaccines, totalCount) = await _vaccineService.GetAllVaccines(pageIndex, pageSize);
                var response = BaseResponse<IEnumerable<VaccineRes>>.OkResponse(vaccines, "Vaccines retrieved successfully");
                response.TotalCount = totalCount; // Giả sử BaseResponse có thuộc tính TotalCount
                return Ok(response);
            }
            catch (Exception e)
            {
                _logger.LogError("Error while fetching all vaccines: {Error}", e.Message);
                return HandleException(e, "Internal Server Error");
            }
        }

        // GET api/v1/vaccine/by-age
        [HttpGet("by-age")]
        public async Task<IActionResult> GetVaccinesByAgeRange(
            [FromQuery] int minAge,
            [FromQuery] int maxAge,
            [FromQuery] string unit)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(unit))
                {
                    return BadRequest(BaseResponse<string>.BadRequestResponse("Unit cannot be empty"));
                }

                _logger.LogInformation("Fetching vaccines for age range: {MinAge}-{MaxAge} {Unit}", minAge, maxAge, unit);
                var vaccines = await _vaccineService.GetAllVaccinesForEachAge(minAge, maxAge, unit);
                return Ok(BaseResponse<IEnumerable<VaccineRes>>.OkResponse(vaccines, "Vaccines retrieved successfully"));
            }
            catch (Exception e)
            {
                _logger.LogError("Error while fetching vaccines by age range: {Error}", e.Message);
                return HandleException(e, "Internal Server Error");
            }
        }

        // GET api/v1/vaccine/by-name
        [HttpGet("by-name")]
        public async Task<IActionResult> GetVaccinesByNameDifferentManufacturers([FromQuery] string vaccineName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(vaccineName))
                {
                    return BadRequest(BaseResponse<string>.BadRequestResponse("Vaccine name cannot be empty"));
                }

                _logger.LogInformation("Fetching vaccines by name: {VaccineName}", vaccineName);
                var vaccines = await _vaccineService.GetVaccinesByNameDifferentManufacturers(vaccineName);
                return Ok(BaseResponse<IEnumerable<VaccineRes>>.OkResponse(vaccines, "Vaccines retrieved successfully"));
            }
            catch (Exception e)
            {
                _logger.LogError("Error while fetching vaccines by name {VaccineName}: {Error}", vaccineName, e.Message);
                return HandleException(e, "Internal Server Error");
            }
        }
        
        [HttpGet("all")]
        public async Task<IActionResult> GetAllVaccine()
        {
            try
            {
                var vaccines = await _vaccineService.GetAllVaccines();
                return Ok(BaseResponse<List<Vaccine>>.OkResponse(vaccines, "Vaccines retrieved successfully"));
            }
            catch (Exception e)
            {
                return HandleException(e, "Internal Server Error");
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
            }
        }
    }
}