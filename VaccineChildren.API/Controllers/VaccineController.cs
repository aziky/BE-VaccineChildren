using Microsoft.AspNetCore.Mvc;
using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Application.DTOs.Response;
using VaccineChildren.Application.Services;
using VaccineChildren.Core.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            _logger = logger;
            _vaccineService = vaccineService;
        }

        // POST api/v1/vaccine
        [HttpPost]
        public async Task<IActionResult> CreateVaccine([FromBody] VaccineReq vaccineReq)
        {
            try
            {
                _logger.LogInformation("Creating new vaccine");
                await _vaccineService.CreateVaccine(vaccineReq);
                return Ok(BaseResponse<List<VaccineRes>>.OkResponse(null, "Vaccine created successfully"));
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
                _logger.LogError("Vaccine not found: {Error}", e.Message);
                return NotFound(BaseResponse<string>.NotFoundResponse("Vaccine not found"));
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
                _logger.LogInformation("Updating vaccine with ID: {VaccineId}", vaccineId);
                await _vaccineService.UpdateVaccine(vaccineId, vaccineReq);
                return Ok(BaseResponse<string>.OkResponse(null, "Vaccine updated successfully"));
            }
            catch (KeyNotFoundException e)
            {
                _logger.LogError("Vaccine not found: {Error}", e.Message);
                return NotFound(BaseResponse<string>.NotFoundResponse("Vaccine not found"));
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
                _logger.LogError("Vaccine not found: {Error}", e.Message);
                return NotFound(BaseResponse<string>.NotFoundResponse("Vaccine not found"));
            }
            catch (Exception e)
            {
                _logger.LogError("Error while deleting vaccine: {Error}", e.Message);
                return HandleException(e, "Internal Server Error");
            }
        }

        // GET api/v1/vaccine
        [HttpGet]
        public async Task<IActionResult> GetAllVaccines()
        {
            try
            {
                _logger.LogInformation("Fetching all vaccines");
                var vaccineList = await _vaccineService.GetAllVaccines();
                return Ok(BaseResponse<List<VaccineRes>>.OkResponse(vaccineList, "Vaccines retrieved successfully"));
            }
            catch (Exception e)
            {
                _logger.LogError("Error while fetching all vaccines: {Error}", e.Message);
                return HandleException(e, "Internal Server Error");
            }
        }

        [HttpGet("9-months-age")]
        public async Task<IActionResult> GetAllVaccines9MonthAge()
        {
            try
            {
                _logger.LogInformation("Retrieving vaccines with MinAge = 0, MaxAge = 9, Unit = 'month'");
                var vaccines = await _vaccineService.GetAllVaccines9MonthAge();
                return Ok(BaseResponse<List<VaccineRes>>.OkResponse(vaccines, "Vaccines retrieved successfully"));
            }
            catch (Exception e)
            {
                _logger.LogError("Error while retrieving vaccines: {Error}", e.Message);
                return HandleException(e, "Internal Server Error");
            }
        }

        [HttpGet("12-months-age")]
        public async Task<IActionResult> GetAllVaccines12MonthAge()
        {
            try
            {
                _logger.LogInformation("Retrieving vaccines with MinAge = 0, MaxAge = 12, Unit = 'month'");
                var vaccines = await _vaccineService.GetAllVaccines12MonthAge();
                return Ok(BaseResponse<List<VaccineRes>>.OkResponse(vaccines, "Vaccines retrieved successfully"));
            }
            catch (Exception e)
            {
                _logger.LogError("Error while retrieving vaccines: {Error}", e.Message);
                return HandleException(e, "Internal Server Error");
            }
        }

        [HttpGet("24-months-age")]
        public async Task<IActionResult> GetAllVaccines24MonthAge()
        {
            try
            {
                _logger.LogInformation("Retrieving vaccines with MinAge = 0, MaxAge = 24, Unit = 'month'");
                var vaccines = await _vaccineService.GetAllVaccines24MonthAge();
                return Ok(BaseResponse<List<VaccineRes>>.OkResponse(vaccines, "Vaccines retrieved successfully"));
            }
            catch (Exception e)
            {
                _logger.LogError("Error while retrieving vaccines: {Error}", e.Message);
                return HandleException(e, "Internal Server Error");
            }
        }

        [HttpGet("4-8-years-age")]
        public async Task<IActionResult> GetAllVaccines4To8YearsAge()
        {
            try
            {
                _logger.LogInformation("Retrieving vaccines with MinAge >= 4, MaxAge <= 8, Unit = 'year'");

                var vaccines = await _vaccineService.GetAllVaccines4To8YearsAge();
                return Ok(BaseResponse<List<VaccineRes>>.OkResponse(vaccines, "Vaccines retrieved successfully"));
            }
            catch (Exception e)
            {
                _logger.LogError("Error while retrieving vaccines: {Error}", e.Message);
                return HandleException(e, "Internal Server Error");
            }
        }

    }
}
