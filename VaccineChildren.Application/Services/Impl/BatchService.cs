using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Application.DTOs.Response;
using VaccineChildren.Domain.Entities;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VaccineChildren.Domain.Abstraction;
using VaccineChildren.Application.DTOs.Requests;
using VaccineChildren.Application.DTOs.Responses;
using VaccineChildren.Core.Exceptions;

public class BatchService : IBatchService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<BatchService> _logger;
    private readonly IGenericRepository<Batch> _batchRepository;

    public BatchService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<BatchService> logger, IGenericRepository<Batch> batchRepository)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _batchRepository = batchRepository;
    }

    // Create a new batch
    public async Task CreateBatch(BatchReq batchReq)
    {
        try
        {
            _logger.LogInformation("Start creating batch");

            var vaccine = await _unitOfWork.GetRepository<Vaccine>().GetByIdAsync(batchReq.VaccineId ?? Guid.Empty);
            if (vaccine == null)
            {
                throw new CustomExceptions.EntityNotFoundException("Vaccine", batchReq.VaccineId);
            }

            var batch = new Batch
            {
                BatchId = Guid.NewGuid(),
                VaccineId = batchReq.VaccineId.Value,
                ProductionDate = batchReq.ProductionDate.Value,
                ExpirationDate = batchReq.ExpirationDate.Value,
                Quantity = batchReq.Quantity.Value,
                IsActive = batchReq.IsActive ?? true
            };

            await _batchRepository.InsertAsync(batch);
            await _unitOfWork.SaveChangeAsync();

            _logger.LogInformation("Batch created successfully");
        }
        catch (Exception e)
        {
            _logger.LogError("Error while creating batch: {Error}", e.Message);
            throw;
        }
    }


    public async Task<BatchRes> GetBatchById(Guid batchId)
    {
        try
        {
            _logger.LogInformation("Retrieving batch with ID: {BatchId}", batchId);

            var batch = await _batchRepository.GetByIdAsync(batchId);
            if (batch == null)
            {
                _logger.LogInformation("Batch not found with ID: {BatchId}", batchId);
                throw new KeyNotFoundException("Batch not found");
            }

            var vaccine = await _unitOfWork.GetRepository<Vaccine>().GetByIdAsync(batch.VaccineId);
            if (vaccine == null)
            {
                _logger.LogInformation("Vaccine associated with batch not found.");
                throw new KeyNotFoundException("Vaccine associated with batch not found");
            }

            var batchRes = _mapper.Map<BatchRes>(batch);

            batchRes.Vaccine = vaccine;

            return batchRes;
        }
        catch (Exception e)
        {
            _logger.LogError("Error while retrieving batch: {Error}", e.Message);
            throw;
        }
    }


    public async Task<List<BatchRes>> GetAllBatchs()
    {
        try
        {
            _logger.LogInformation("Retrieving all batches.");

            var batches = await _batchRepository.GetAllAsync(query => query
                .Where(b => b.IsActive == true));
            if (batches == null || batches.Count == 0)
            {
                _logger.LogInformation("No batches found.");
                return new List<BatchRes>();
            }

            var batchResList = new List<BatchRes>();

            foreach (var batch in batches)
            {
                var vaccine = await _unitOfWork.GetRepository<Vaccine>().GetByIdAsync(batch.VaccineId);
                if (vaccine == null)
                {
                    _logger.LogInformation("Vaccine associated with batch {BatchId} not found.", batch.BatchId);
                    continue; 
                }

                var batchRes = _mapper.Map<BatchRes>(batch);

                batchRes.Vaccine = vaccine;

                batchResList.Add(batchRes);
            }

            return batchResList;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while retrieving batches: {Error}", ex.Message);
            throw;
        }
    }


    // Update a batch by ID
    public async Task UpdateBatch(Guid batchId, BatchReq batchReq)
    {
        try
        {
            _logger.LogInformation("Start updating batch with ID: {BatchId}", batchId);

            // Retrieve the batch to update
            var batch = await _batchRepository.GetByIdAsync(batchId);
            if (batch == null)
            {
                _logger.LogInformation("Batch not found with ID: {BatchId}", batchId);
                throw new KeyNotFoundException($"Batch with ID {batchId} not found.");
            }

            // Optionally: Validation for the batch data if needed, e.g., check if production date is valid
            if (batchReq.ProductionDate.HasValue && batchReq.ExpirationDate.HasValue && batchReq.ProductionDate > batchReq.ExpirationDate)
            {
                throw new CustomExceptions.ValidationException("Production date cannot be later than expiration date.");
            }

            // Map updated properties from BatchReq to the existing Batch entity
            _mapper.Map(batchReq, batch);

            // Ensure that VaccineId is valid, if provided
            if (batchReq.VaccineId.HasValue)
            {
                var vaccine = await _unitOfWork.GetRepository<Vaccine>().GetByIdAsync(batchReq.VaccineId.Value);
                if (vaccine == null)
                {
                    _logger.LogError("Vaccine with ID {VaccineId} not found.", batchReq.VaccineId.Value);
                    throw new KeyNotFoundException($"Vaccine with ID {batchReq.VaccineId} not found.");
                }
            }

            // Update the batch and save changes to the database
            _batchRepository.UpdateAsync(batch);
            _unitOfWork.CommitTransaction();

            _logger.LogInformation("Batch updated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while updating batch: {Error}", ex.Message);
            throw;
        }
    }


    // Delete a batch by ID
    public async Task DeleteBatch(Guid batchId)
    {
        try
        {
            _logger.LogInformation("Start deleting batch with ID: {BatchId}", batchId);

            var batch = await _batchRepository.GetByIdAsync(batchId);
            if (batch == null)
            {
                _logger.LogInformation("Batch not found with ID: {BatchId}", batchId);
                throw new KeyNotFoundException($"Batch with ID {batchId} not found.");
            }

            // Mark the batch as inactive instead of deleting
            batch.IsActive = false;

            // Update the batch status
            _batchRepository.UpdateAsync(batch);
            _unitOfWork.CommitTransaction();

            _logger.LogInformation("Batch deleted successfully (marked as inactive).");
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while deleting batch: {Error}", ex.Message);
            throw;
        }
    }

}
