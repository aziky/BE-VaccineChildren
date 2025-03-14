using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Application.DTOs.Response;
using VaccineChildren.Application.Services;
using VaccineChildren.Core.Base;

namespace VaccineChildren.API.Controllers
{
    [Route("api/v1/package")]
    [ApiController]
    [Authorize (Roles = "manager")]
    public class PackageController : BaseController
    {
        private readonly ILogger<PackageController> _logger;
        private readonly IPackageService _packageService;

        public PackageController(ILogger<PackageController> logger, IPackageService packageService)
        {
            _logger = logger;
            _packageService = packageService;
        }

        // POST api/v1/package
        [HttpPost]
        public async Task<IActionResult> CreatePackage([FromBody] PackageReq packageReq)
        {
            try
            {
                _logger.LogInformation("Creating new package");
                await _packageService.CreatePackage(packageReq);
                return Ok(BaseResponse<string>.OkResponse(null, "Package created successfully"));
            }
            catch (Exception e)
            {
                _logger.LogError("Error while creating package: {Error}", e.Message);
                return HandleException(e, "Internal Server Error");
            }
        }

        // GET api/v1/package/{packageId}
        [HttpGet("{packageId:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPackageById(Guid packageId)
        {
            try
            {
                _logger.LogInformation("Fetching package by ID: {PackageId}", packageId);
                var package = await _packageService.GetPackageById(packageId);
                return Ok(BaseResponse<PackageRes>.OkResponse(package, "Package retrieved successfully"));
            }
            catch (KeyNotFoundException e)
            {
                _logger.LogError("Package not found: {Error}", e.Message);
                return NotFound(BaseResponse<string>.NotFoundResponse("Package not found"));
            }
            catch (Exception e)
            {
                _logger.LogError("Error while fetching package: {Error}", e.Message);
                return HandleException(e, "Internal Server Error");
            }
        }

        // PUT api/v1/package/{packageId}
        [HttpPut("{packageId:guid}")]
        public async Task<IActionResult> UpdatePackage(Guid packageId, [FromBody] PackageReq packageReq)
        {
            try
            {
                _logger.LogInformation("Updating package with ID: {PackageId}", packageId);
                await _packageService.UpdatePackage(packageId, packageReq);
                return Ok(BaseResponse<string>.OkResponse(null, "Package updated successfully"));
            }
            catch (KeyNotFoundException e)
            {
                _logger.LogError("Package not found: {Error}", e.Message);
                return NotFound(BaseResponse<string>.NotFoundResponse("Package not found"));
            }
            catch (Exception e)
            {
                _logger.LogError("Error while updating package: {Error}", e.Message);
                return HandleException(e, "Internal Server Error");
            }
        }

        // PUT api/v1/package/{packageId}/update-vaccines
        [HttpPut("{packageId:guid}/update-vaccines")]
        public async Task<IActionResult> UpdatePackageVaccines(Guid packageId, [FromBody] UpdatePackageVaccinesReq request)
        {
            try
            {
                _logger.LogInformation("Updating vaccines for package with ID: {PackageId}", packageId);

                // Kiểm tra request đầu vào
                if (request?.VaccineIds == null || request.VaccineIds.Count == 0)
                {
                    return BadRequest(BaseResponse<string>.BadRequestResponse("VaccineIds cannot be empty"));
                }

                // Gọi service để cập nhật vaccine
                var updatedPackage = await _packageService.UpdatePackageVaccines(packageId, request);

                return Ok(BaseResponse<PackageRes>.OkResponse(updatedPackage, "Package updated successfully"));
            }
            catch (KeyNotFoundException e)
            {
                _logger.LogError(e, "Package not found: {Message}", e.Message);
                return NotFound(BaseResponse<string>.NotFoundResponse(e.Message));
            }
            catch (InvalidOperationException e)
            {
                _logger.LogError(e, "Invalid operation: {Message}", e.Message);
                return BadRequest(BaseResponse<string>.BadRequestResponse(e.Message));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected error while updating package: {Message}", e.Message);
                return HandleException(e, "Internal Server Error");
            }
        }
        

        // DELETE api/v1/package/{packageId}
        [HttpDelete("{packageId:guid}")]
        public async Task<IActionResult> DeletePackage(Guid packageId)
        {
            try
            {
                _logger.LogInformation("Deleting package with ID: {PackageId}", packageId);
                await _packageService.DeletePackage(packageId);
                return Ok(BaseResponse<string>.OkResponse(null, "Package deleted successfully"));
            }
            catch (KeyNotFoundException e)
            {
                _logger.LogError("Package not found: {Error}", e.Message);
                return NotFound(BaseResponse<string>.NotFoundResponse("Package not found"));
            }
            catch (Exception e)
            {
                _logger.LogError("Error while deleting package: {Error}", e.Message);
                return HandleException(e, "Internal Server Error");
            }
        }

        // GET api/v1/package
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllPackages()
        {
            try
            {
                _logger.LogInformation("Fetching all packages");
                var packageList = await _packageService.GetAllPackages();
                return Ok(BaseResponse<List<PackageRes>>.OkResponse(packageList, "Packages retrieved successfully"));
            }
            catch (Exception e)
            {
                _logger.LogError("Error while fetching all packages: {Error}", e.Message);
                return HandleException(e, "Internal Server Error");
            }
        }
        
        
    }
}
