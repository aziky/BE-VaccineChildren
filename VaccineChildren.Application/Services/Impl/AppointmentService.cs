using System.Globalization;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Core.Store;
using VaccineChildren.Domain.Abstraction;
using VaccineChildren.Domain.Entities;

namespace VaccineChildren.Application.Services.Impl;

public class AppointmentService : IAppointmentService
{
    private const string DateFormat = "yyyy-MM-dd";
    private readonly ILogger<AppointmentService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AppointmentService(ILogger<AppointmentService> logger, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task CreateAppointmentAsync(CreateAppointmentReq request)
    {
        try
        {
            _logger.LogInformation("Start creating child");
            _unitOfWork.BeginTransaction();
            var childRepository = _unitOfWork.GetRepository<Child>();
            var userRepository = _unitOfWork.GetRepository<User>();
            var packageRepository = _unitOfWork.GetRepository<Package>();
            var vaccineManuRepository = _unitOfWork.GetRepository<VaccineManufacture>();
            var orderRepository = _unitOfWork.GetRepository<Order>();

            var user = await userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogError("User not found");
                throw new KeyNotFoundException("User not found");
            }

            _logger.LogInformation("Start creating child");
            var child = _mapper.Map<Child>(request);
            child.Dob = DateOnly.ParseExact(request.Dob, DateFormat, CultureInfo.InvariantCulture);
            user.Children.Add(await childRepository.InsertAsync(child));

            _logger.LogInformation("Start creating order");
            var order = new Order
            {
                ChildId = child.ChildId,
                OrderDate = DateTime.ParseExact(request.InjectionDate, DateFormat, CultureInfo.InvariantCulture),
                Status = StaticEnum.OrderStatusEnum.Processing.Name(),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = user.FullName,
                Packages = await packageRepository.GetAllAsync(query => query
                    .Where(p => request.PackageIdList.Contains(p.PackageId.ToString()))),
                Vaccines = await vaccineManuRepository.GetAllAsync(query => query
                    .Where(vm => request.VaccineIdList.Contains(vm.VaccineId.ToString())))
            };
            
            await orderRepository.InsertAsync(order);

            await _unitOfWork.SaveChangeAsync();
             _unitOfWork.CommitTransaction();

        }
        catch (Exception e)
        {
            _unitOfWork.RollBack();
            _logger.LogError("Error at create child async cause by {}", e.Message);
            throw;
        }
        finally
        {
            _unitOfWork.Dispose();
        }
    }
    
}