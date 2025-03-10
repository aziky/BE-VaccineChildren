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

    public async Task<IList<VaccinatedHistory>> GetVaccinatedHistory(Guid orderId)
    {
        try
        {
            _logger.LogInformation("Start retrieving vaccinated history with paymentId {}", orderId);
            var scheduleRepository = _unitOfWork.GetRepository<Schedule>();
            var orderRepository = _unitOfWork.GetRepository<Order>();
            var scheduleList = await scheduleRepository.GetAllAsync(query => query
                .Include(o => o.Order)
                .Include(s => s.Child)
                .Where(s => s.OrderId == orderId)
            );
            var response = _mapper.Map<IList<VaccinatedHistory>>(scheduleList);

            var packageModified = scheduleList.FirstOrDefault().Order.PackageModified;
            // if (packageModified == null)
            // {
            //     _logger.LogInformation("Start mapping vaccines");
            //     var order = await orderRepository.GetAllAsync(query => query
            //         .Include(o => o.Vaccines).ThenInclude(vm => vm.Manufacturer)
            //         .Where(o => o.OrderId == orderId)
            //     );
            //     var vaccineList = order.SelectMany(o => o.Vaccines).ToList();
            //
            //     foreach (var history in response)
            //     {
            //         var matchedVaccine = vaccineList.FirstOrDefault(v => v.VaccineId.ToString() == history.VaccineId);
            //         if (matchedVaccine != null)
            //         {
            //             history.ManufacturerName = matchedVaccine.Manufacturer.Name;
            //         }
            //     }
            // }
            // else if (packageModified == false)
            // {
            //     _logger.LogInformation("Start mapping package");
            //     var order = await orderRepository.GetAllAsync(query => query
            //         .Include(o => o.Packages).ThenInclude(p => p.Vaccines)
            //         .ThenInclude(v => v.VaccineManufactures).ThenInclude(vm => vm.Manufacturer)
            //         .Where(o => o.OrderId == orderId)
            //     );
            //
            //     var vaccineList = order
            //         .SelectMany(o => o.Packages)
            //         .SelectMany(p => p.Vaccines)
            //         .SelectMany(v => v.VaccineManufactures) // Get many-to-many VaccineManufactures
            //         .ToList();
            //
            //     foreach (var history in response)
            //     {
            //         var matchedVaccine = vaccineList.FirstOrDefault(vm => vm.VaccineId.ToString() == history.VaccineId);
            //         if (matchedVaccine?.Manufacturer != null)
            //         {
            //             history.ManufacturerName = matchedVaccine.Manufacturer.Name;
            //         }
            //     }
            // }
            // else
            // {
            //     _logger.LogInformation("Start mapping modify package");
            //     var order = await orderRepository.GetAllAsync(query => query
            //         .Include(o => o.Packages)
            //         .Where(o => o.OrderId == orderId)
            //     );
            //     var userCartRepository = _unitOfWork.GetRepository<UserCart>();
            //     var packageList = await userCartRepository.GetAllAsync(query => query
            //         .Include(uc => uc.Vaccine)
            //         .ThenInclude(v => v.VaccineManufactures)
            //         .ThenInclude(vm => vm.Manufacturer)
            //         .Where(u => u.PackageId == order.FirstOrDefault().OrderId && u.ChildId == order.FirstOrDefault().ChildId)
            //     );
            //     
            //     var vaccineList = packageList
            //         .Select(u => u.Vaccine)
            //         .SelectMany(v => v.VaccineManufactures) 
            //         .ToList();
            //
            //     foreach (var history in response)
            //     {
            //         var matchedVaccine = vaccineList.FirstOrDefault(vm => vm.VaccineId.ToString() == history.VaccineId);
            //         if (matchedVaccine?.Manufacturer != null)
            //         {
            //             history.ManufacturerName = matchedVaccine.Manufacturer.Name;
            //         }
            //     }
            // }

            return response;
        }
        catch (Exception e)
        {
            _logger.LogError("Error at get vaccinated history cause by {}", e.Message);
            throw;
        }
    }
}