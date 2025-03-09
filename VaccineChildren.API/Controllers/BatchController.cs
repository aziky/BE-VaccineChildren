using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VaccineChildren.Application.DTOs.Requests;
using VaccineChildren.Application.DTOs.Response;
using VaccineChildren.Application.DTOs.Responses;
using VaccineChildren.Application.Services;
using VaccineChildren.Core.Base;

namespace VaccineChildren.API.Controllers
{
    [Route("api/v1/batch")]
    [ApiController]
    [Authorize (Roles = "manager")]
    public class BatchController : ControllerBase
    {
        private readonly IBatchService _batchService;
        private readonly ILogger<BatchController> _logger;

        public BatchController(IBatchService batchService, ILogger<BatchController> logger)
        {
            _batchService = batchService;
            _logger = logger;
        }

        // POST api/v1/batch
        [HttpPost]
        public async Task<IActionResult> CreateBatch([FromBody] BatchReq batchReq)
        {
            try
            {
                if (batchReq == null)
                {
                    return BadRequest(BaseResponse<string>.BadRequestResponse("Batch data is required."));
                }

                _logger.LogInformation("Creating new batch");
                await _batchService.CreateBatch(batchReq);
                return Ok(BaseResponse<string>.OkResponse(null, "Batch created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while creating batch: {Error}", ex.Message);
                return StatusCode(500, BaseResponse<string>.ErrorResponse("Internal Server Error", ex.Message));
            }
        }

        // GET api/v1/batch/{batchId}
        [HttpGet("{batchId}")]
        public async Task<IActionResult> GetBatchById(string batchId)
        {
            try
            {
                _logger.LogInformation("Fetching batch by ID: {BatchId}", batchId);
                var batchRes = await _batchService.GetBatchById(batchId);
                return Ok(BaseResponse<BatchRes>.OkResponse(batchRes, "Batch retrieved successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError("Batch not found: {Error}", ex.Message);
                return NotFound(BaseResponse<string>.NotFoundResponse("Batch not found"));
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while fetching batch: {Error}", ex.Message);
                return StatusCode(500, BaseResponse<string>.ErrorResponse("Internal Server Error", ex.Message));
            }
        }

        // GET api/v1/batch
        [HttpGet]
        public async Task<IActionResult> GetAllBatches()
        {
            try
            {
                _logger.LogInformation("Fetching all batches");
                var batches = await _batchService.GetAllBatches();
                return Ok(BaseResponse<List<BatchRes>>.OkResponse(batches, "Batches retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while fetching batches: {Error}", ex.Message);
                return StatusCode(500, BaseResponse<string>.ErrorResponse("Internal Server Error", ex.Message));
            }
        }

        // PUT api/v1/batch/{batchId}
        [HttpPut("{batchId}")]
        public async Task<IActionResult> UpdateBatch(string batchId, [FromBody] BatchReq batchReq)
        {
            try
            {
                if (batchReq == null)
                {
                    return BadRequest(BaseResponse<string>.BadRequestResponse("Batch data is required."));
                }

                _logger.LogInformation("Updating batch with ID: {BatchId}", batchId);
                await _batchService.UpdateBatch(batchId, batchReq);
                return Ok(BaseResponse<string>.OkResponse(null, "Batch updated successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError("Batch not found: {Error}", ex.Message);
                return NotFound(BaseResponse<string>.NotFoundResponse("Batch not found"));
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while updating batch: {Error}", ex.Message);
                return StatusCode(500, BaseResponse<string>.ErrorResponse("Internal Server Error", ex.Message));
            }
        }

        // DELETE api/v1/batch/{batchId}
        [HttpDelete("{batchId}")]
        public async Task<IActionResult> DeleteBatch(string batchId)
        {
            try
            {
                _logger.LogInformation("Deleting batch with ID: {BatchId}", batchId);
                await _batchService.DeleteBatch(batchId);
                return Ok(BaseResponse<string>.OkResponse(null, "Batch marked as inactive (deleted)."));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError("Batch not found: {Error}", ex.Message);
                return NotFound(BaseResponse<string>.NotFoundResponse("Batch not found"));
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while deleting batch: {Error}", ex.Message);
                return StatusCode(500, BaseResponse<string>.ErrorResponse("Internal Server Error", ex.Message));
            }
        }
    }
}
