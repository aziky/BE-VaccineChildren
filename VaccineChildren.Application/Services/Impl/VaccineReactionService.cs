using System.Text.Json;
using Microsoft.Extensions.Logging;
using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Application.DTOs.Response;
using VaccineChildren.Application.Services;
using VaccineChildren.Core.Store;
using VaccineChildren.Domain.Abstraction;
using VaccineChildren.Domain.Entities;

public class VaccineReactionService : IVaccineReactionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<VaccineReactionService> _logger;

    public VaccineReactionService(
        IUnitOfWork unitOfWork,
        ILogger<VaccineReactionService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<VaccineReactionResponse> RecordVaccineReactionAsync(VaccineReactionRequest request)
    {
        try
        {
            _logger.LogInformation("Recording vaccine reaction for schedule {ScheduleId}", request.ScheduleId);

            // Kiểm tra lịch tiêm tồn tại
            var scheduleRepository = _unitOfWork.GetRepository<Schedule>();
            var childRepository = _unitOfWork.GetRepository<Child>();

            var schedule = await scheduleRepository.FindByConditionAsync(s => s.ScheduleId == request.ScheduleId);
            if (schedule == null)
            {
                throw new Exception($"Schedule with ID {request.ScheduleId} not found");
            }

            // Lấy thông tin trẻ
            var child = await childRepository.FindByConditionAsync(c => c.ChildId == schedule.ChildId);
            if (child == null)
            {
                throw new Exception($"Child not found for schedule {request.ScheduleId}");
            }
            
            var reactionRepository = _unitOfWork.GetRepository<VaccineReaction>();
            var reaction = new VaccineReaction
            {
                ReactionId = Guid.NewGuid(),
                ScheduleId = request.ScheduleId,
                ReactionDescription = JsonSerializer.Serialize(new
                {
                    reactions = request.Reactions,
                    otherReactions = request.OtherReactions,
                    notes = request.Notes
                }),
                Severity = request.Severity,
                OnsetTime = DateTime.UtcNow.ToLocalTime(),
                CreatedAt = DateTime.UtcNow.ToLocalTime(),
                ResolvedTime = DateTime.UtcNow.ToLocalTime()
                // CreatedBy = _currentUserService.GetUserName() ?? "System"
            };

            await reactionRepository.InsertAsync(reaction);
            await _unitOfWork.SaveChangeAsync();

            // Cập nhật lịch trình tiêm nếu có trạng thái
            if (schedule.status == StaticEnum.ScheduleStatusEnum.Vaccinated.Name())
            {
                schedule.status = StaticEnum.ScheduleStatusEnum.Completed.Name();
                schedule.UpdatedAt = DateTime.UtcNow.ToLocalTime();
                // schedule.UpdatedBy = _currentUserService.GetUserName() ?? "System";


                await _unitOfWork.SaveChangeAsync();
            }

            // Trả về response
            return new VaccineReactionResponse
            {
                ReactionId = reaction.ReactionId,
                ScheduleId = request.ScheduleId,
                ChildName = child.FullName,
                VaccineId = schedule.VaccineId,
                Reactions = request.Reactions,
                OtherReactions = request.OtherReactions,
                Severity = request.Severity,
                OnsetTime = reaction.OnsetTime.Value,
                Notes = request.Notes
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording vaccine reaction for schedule {ScheduleId}", request.ScheduleId);
            throw;
        }
    }

    // public async Task<List<VaccineReactionResponse>> GetVaccineReactionsForScheduleAsync(Guid scheduleId)
    // {
    //     try
    //     {
    //         _logger.LogInformation("Getting vaccine reactions for schedule {ScheduleId}", scheduleId);
    //
    //         var scheduleRepository = _unitOfWork.GetRepository<Schedule>();
    //         var childRepository = _unitOfWork.GetRepository<Child>();
    //         var reactionRepository = _unitOfWork.GetRepository<VaccineReaction>();
    //
    //         var schedule = await scheduleRepository.FindByConditionAsync(s => s.ScheduleId == scheduleId);
    //         if (schedule == null)
    //         {
    //             throw new Exception($"Schedule with ID {scheduleId} not found");
    //         }
    //
    //         var child = await childRepository.FindByConditionAsync(c => c.ChildId == schedule.ChildId);
    //
    //         // Lấy danh sách phản ứng
    //         var reactions = await reactionRepository.GetAllAsync(query =>
    //             query.Where(r => r.ScheduleId == scheduleId)
    //                 .OrderByDescending(r => r.OnsetTime));
    //
    //         var result = new List<VaccineReactionResponse>();
    //
    //         foreach (var reaction in reactions)
    //         {
    //             var reactionData = JsonSerializer.Deserialize<dynamic>(reaction.ReactionDescription);
    //
    //             result.Add(new VaccineReactionResponse
    //             {
    //                 ReactionId = reaction.ReactionId,
    //                 ScheduleId = scheduleId,
    //                 ChildName = child?.FullName ?? "Unknown",
    //                 VaccineType = schedule.VaccineType,
    //                 Reactions = reactionData.GetProperty("reactions").EnumerateArray().Select(r => r.GetString())
    //                     .ToList(),
    //                 OtherReactions = reactionData.GetProperty("otherReactions").GetString(),
    //                 Severity = reaction.Severity,
    //                 OnsetTime = reaction.OnsetTime.Value,
    //                 Notes = reactionData.GetProperty("notes").GetString()
    //             });
    //         }
    //
    //         return result;
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Error getting vaccine reactions for schedule {ScheduleId}", scheduleId);
    //         throw;
    //     }
    // }
}