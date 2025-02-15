using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Application.DTOs.Requests;
using VaccineChildren.Application.DTOs.Response;
using VaccineChildren.Application.DTOs.Responses;
using VaccineChildren.Domain.Entities;

public interface IBatchService
{
    Task CreateBatch(BatchReq batchReq);
    Task<BatchRes?> GetBatchById(Guid batchId);
    Task<List<BatchRes>> GetAllBatchs();
    Task UpdateBatch(Guid batchId, BatchReq batchReq);
    Task DeleteBatch(Guid batchId);
}
