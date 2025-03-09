using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Application.DTOs.Response;

public interface IVaccineReactionService
{
    Task<VaccineReactionResponse> RecordVaccineReactionAsync(VaccineReactionRequest request);
    // Task<List<VaccineReactionResponse>> GetVaccineReactionsForScheduleAsync(Guid scheduleId);
}