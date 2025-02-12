using System.Globalization;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Core.Store;
using VaccineChildren.Domain.Abstraction;
using VaccineChildren.Domain.Entities;

namespace VaccineChildren.Application.Services.Impl;

public class OrderService : IOrderService
{
    private const string DateFormat = "dd-MM-yyyy";
    private readonly ILogger<OrderService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public OrderService(ILogger<OrderService> logger, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task CreateOrderAsync(CreateOrderReq request)
    {
        try
        {
            _logger.LogInformation("Start creating appointment");
            _unitOfWork.BeginTransaction();
            var childRepository = _unitOfWork.GetRepository<Child>();
            var userRepository = _unitOfWork.GetRepository<User>();
            var packageRepository = _unitOfWork.GetRepository<Package>();
            var vaccineManuRepository = _unitOfWork.GetRepository<VaccineManufacture>();
            var orderRepository = _unitOfWork.GetRepository<Order>();
            var userCartRepository = _unitOfWork.GetRepository<UserCart>();
            var paymentRepository = _unitOfWork.GetRepository<Payment>();

            var user = await userRepository.GetByIdAsync(Guid.Parse(request.UserId));
            if (user == null)
            {
                _logger.LogError("User not found");
                throw new KeyNotFoundException("User not found");
            }

            _logger.LogInformation("Start creating child");
            var child = _mapper.Map<Child>(request);
            child.Dob = DateOnly.ParseExact(request.Dob, DateFormat, CultureInfo.InvariantCulture);
            child.CreatedAt = DateTime.Now;
            child.CreatedBy = user.Email;
            
            user.Children.Add(await childRepository.InsertAsync(child));

            _logger.LogInformation("Start creating order");
            
            var noModifiedPackages = request.PackageList.Where(p => p.Modified == false).Select(p => p.PackageId).ToList();
            var order = new Order
            {
                ChildId = child.ChildId,
                OrderDate = DateTime.ParseExact(request.InjectionDate + " 00:00:00", "dd-MM-yyyy HH:mm:ss",
                    CultureInfo.InvariantCulture),
                Status = StaticEnum.OrderStatusEnum.Processing.Name(),
                PackageModified = request.PackageList.IsNullOrEmpty() ? null : request.PackageList.Any(p => p.Modified),
                Packages = await packageRepository.GetAllAsync(query => query
                    .Where(p => noModifiedPackages.Contains(p.PackageId.ToString()))),
                CreatedAt = DateTime.Now,
                CreatedBy = user.FullName,
                Vaccines = await vaccineManuRepository.GetAllAsync(query => query
                    .Where(vm => request.VaccineIdList.Contains(vm.VaccineId.ToString())))
            };

            _logger.LogInformation("Start creating user cart");
            var userCartList = new List<UserCart>();
            foreach (var package in request.PackageList)
            {
                if (package.Modified)
                {
                    foreach (var vaccineId in package.VaccineModifiedIdList)
                    {
                        userCartList.Add(new UserCart
                        {
                            ChildId = child.ChildId,
                            PackageId = Guid.Parse(package.PackageId),
                            VaccineId = Guid.Parse(vaccineId)
                        });
                    }
                }
            }
            
            if (userCartList.Count != 0) await userCartRepository.InsertRangeAsync(userCartList);
            await orderRepository.InsertAsync(order);
            
            var payment = new Payment
            {
                OrderId = order.OrderId,
                UserId = user.UserId,
                Amount = (decimal) request.Amount,
                PaymentMethod = StaticEnum.PaymentMethodEnum.VnPay.Name(),
                PaymentStatus = StaticEnum.PaymentStatusEnum.Pending.Name(),
                CreatedAt = order.CreatedAt,
                CreatedBy = order.CreatedBy
            };

            await paymentRepository.InsertAsync(payment);

            await _unitOfWork.SaveChangeAsync();
             _unitOfWork.CommitTransaction();
            _logger.LogInformation("Done creating order async");
        }
        catch (Exception e)
        {
            _unitOfWork.RollBack();
            _logger.LogError("Error at create order async cause by {}", e.Message);
            throw;
        }
        finally
        {
            _unitOfWork.Dispose();
        }
    }
    
}