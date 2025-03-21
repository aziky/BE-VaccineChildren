using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Application.DTOs.Response;

namespace VaccineChildren.Application.Services;

public interface IVaccineReactionService
{
    Task<VaccineReactionResponse> RecordVaccineReactionAsync(VaccineReactionRequest request);
    
}