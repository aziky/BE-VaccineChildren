using AutoMapper;
using Microsoft.Extensions.Logging;
using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Domain.Abstraction;
using VaccineChildren.Domain.Entities;
using VaccineChildren.Application.DTOs.Requests;
using VaccineChildren.Application.DTOs.Response;
using VaccineChildren.Application.DTOs.Responses;
using VaccineChildren.Core.Exceptions;

public class BatchService : IBatchService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<BatchService> _logger;
    private readonly IGenericRepository<Batch> _batchRepository;

    public BatchService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<BatchService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _batchRepository = _unitOfWork.GetRepository<Batch>();
    }

    // 1. Create Batch
    public async Task CreateBatch(BatchReq batchReq)
    {
        try
        {
            _logger.LogInformation("Creating new batch");

            var vaccine = await _unitOfWork.GetRepository<Vaccine>().GetByIdAsync(batchReq.VaccineId ?? Guid.Empty);
            if (vaccine == null)
            {
                throw new CustomExceptions.EntityNotFoundException("Vaccine", batchReq.VaccineId);
            }

            var batch = new Batch
            {
                BatchId = batchReq.BatchId, 
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

    // 2. Get Batch By Id
    public async Task<BatchRes> GetBatchById(String batchId)
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

            // Map Batch to BatchRes
            var batchRes = _mapper.Map<BatchRes>(batch);

            // Use AutoMapper to map Vaccine to VaccineRes (inside the BatchRes)
            batchRes.Vaccine = _mapper.Map<VaccineRes>(vaccine);

            return batchRes;
        }
        catch (Exception e)
        {
            _logger.LogError("Error while retrieving batch: {Error}", e.Message);
            throw;
        }
    }

    // 3. Get All Batches
    public async Task<List<BatchRes>> GetAllBatches()
    {
        try
        {
            _logger.LogInformation("Retrieving all batches");

            var batches = await _batchRepository.GetAllAsync();

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

                // Sử dụng AutoMapper để ánh xạ Vaccine thành VaccineRes
                var batchRes = _mapper.Map<BatchRes>(batch);
                batchRes.Vaccine = _mapper.Map<VaccineRes>(vaccine);

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


    // 4. Update Batch
    public async Task UpdateBatch(String batchId, BatchReq batchReq)
    {
        try
        {
            _logger.LogInformation("Updating batch with ID: {BatchId}", batchId);

            var batch = await _batchRepository.GetByIdAsync(Guid.Parse(batchId));
            if (batch == null)
            {
                _logger.LogInformation("Batch not found with ID: {BatchId}", batchId);
                throw new KeyNotFoundException("Batch not found");
            }

            // Optionally: Validation for batch data if needed (e.g., check if production date is valid)
            if (batchReq.ProductionDate.HasValue && batchReq.ExpirationDate.HasValue && batchReq.ProductionDate > batchReq.ExpirationDate)
            {
                throw new CustomExceptions.ValidationException("Production date cannot be later than expiration date.");
            }

            _mapper.Map(batchReq, batch);

            if (batchReq.VaccineId.HasValue)
            {
                var vaccine = await _unitOfWork.GetRepository<Vaccine>().GetByIdAsync(batchReq.VaccineId.Value);
                if (vaccine == null)
                {
                    _logger.LogError("Vaccine with ID {VaccineId} not found.", batchReq.VaccineId.Value);
                    throw new KeyNotFoundException($"Vaccine with ID {batchReq.VaccineId} not found.");
                }
            }

            _batchRepository.UpdateAsync(batch);
            await _unitOfWork.SaveChangeAsync();

            _logger.LogInformation("Batch updated successfully");
        }
        catch (Exception e)
        {
            _logger.LogError("Error while updating batch: {Error}", e.Message);
            throw;
        }
    }

    // 5. Delete Batch
    public async Task DeleteBatch(String batchId)
    {
        try
        {
            _logger.LogInformation("Deleting batch with ID: {BatchId}", batchId);

            var batch = await _batchRepository.GetByIdAsync(batchId);
            if (batch == null)
            {
                _logger.LogInformation("Batch not found with ID: {BatchId}", batchId);
                throw new KeyNotFoundException("Batch not found");
            }

            batch.IsActive = false; // Mark as inactive (soft delete)

            await _batchRepository.UpdateAsync(batch);
            await _unitOfWork.SaveChangeAsync();

            _logger.LogInformation("Batch deleted successfully (marked as inactive)");
        }
        catch (Exception e)
        {
            _logger.LogError("Error while deleting batch: {Error}", e.Message);
            throw;
        }
    }
}
