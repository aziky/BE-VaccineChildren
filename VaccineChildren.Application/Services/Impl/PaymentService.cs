using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VaccineChildren.Application.DTOs.Response;
using VaccineChildren.Core.Store;
using VaccineChildren.Domain.Abstraction;
using VaccineChildren.Domain.Entities;

namespace VaccineChildren.Application.Services.Impl;

public class PaymentService : IPaymentService
{
    private readonly ILogger<IPaymentService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PaymentService(IUnitOfWork unitOfWork,
        ILogger<IPaymentService> logger,
        IMapper mapper)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }


    public async Task<IList<PaymentHistoryRes>> GetPaymentHistory(Guid userId)
    {
        try
        {
            _logger.LogInformation("Start retrieving payment history");
            var paymentRepository = _unitOfWork.GetRepository<Payment>();
            var payment = await paymentRepository.GetAllAsync(query => query
                .Include(p => p.Order).ThenInclude(o => o.Child)
                .Where(p => p.UserId == userId &&
                            p.PaymentStatus == StaticEnum.PaymentStatusEnum.Paid.Name()));
            var response = _mapper.Map<IList<PaymentHistoryRes>>(payment);
            _logger.LogInformation("Retrieved payment history successfully");
            return response;
        }
        catch (Exception e)
        {
            _logger.LogError("Error at payment history {}", e.Message);
            throw;
        }
    }

    public async Task<IList<VaccinatedHistory>> GetVaccinatedHistory(Guid childId)
    {
        try
        {
            _logger.LogInformation("Start retrieving vaccinated history with paymentId {}", childId);
            var scheduleRepository = _unitOfWork.GetRepository<Schedule>();
            var scheduleList = await scheduleRepository.GetAllAsync(query => query
                .Include(s => s.Vaccine).ThenInclude(v => v.VaccineManufactures).ThenInclude(vm => vm.Manufacturer)
                .Include(s => s.Child)
                .Where(s => s.ChildId == childId && s.status == StaticEnum.ScheduleStatusEnum.Upcoming.Name())
            );
            var response = _mapper.Map<IList<VaccinatedHistory>>(scheduleList);
            

            return response;
        }
        catch (Exception e)
        {
            _logger.LogError("Error at get vaccinated history cause by {}", e.Message);
            throw;
        }
    }
}