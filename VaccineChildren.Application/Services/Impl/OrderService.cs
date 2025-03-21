using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using VaccineChildren.Application.DTOs.Request;
using VaccineChildren.Core.Exceptions;
using VaccineChildren.Core.Store;
using VaccineChildren.Domain.Abstraction;
using VaccineChildren.Domain.Entities;
using VaccineChildren.Domain.Models;
using VaccineChildren.Domain.Models.Momo;

namespace VaccineChildren.Application.Services.Impl;

public class OrderService : IOrderService
{
    private readonly ILogger<OrderService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IVnPayService _vnPayService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ICacheService _cacheService;
    private readonly IEmailService _emailService;
    private readonly IMomoService _momoService;

    public OrderService(ILogger<OrderService> logger, IUnitOfWork unitOfWork, IVnPayService vnPayService,
        IServiceProvider serviceProvider, ICacheService cacheService, IEmailService emailService, 
        IMomoService momoService)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _vnPayService = vnPayService;
        _serviceProvider = serviceProvider;
        _cacheService = cacheService;
        _emailService = emailService;
        _momoService = momoService;
    }

    public async Task<string> CreateOrderAsync(CreateOrderReq request, HttpContext httpContext)
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

            var user = await userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogError("User not found");
                throw new KeyNotFoundException("User not found");
            }

            var child = await childRepository.GetByIdAsync(request.ChildId);

            _logger.LogInformation("Start creating order");
            var formatInjectDate = DateTime.ParseExact(request.InjectionDate + " 00:00:00", "dd-MM-yyyy HH:mm:ss",
                CultureInfo.InvariantCulture);
            var noModifiedPackages =
                request.PackageList.Where(p => p.Modified == false).Select(p => p.PackageId).ToList();
            var order = new Order
            {
                ChildId = child.ChildId,
                OrderDate = formatInjectDate,
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
                Amount = (decimal)request.Amount,
                PaymentDate = DateTime.Now,
                PaymentStatus = StaticEnum.PaymentStatusEnum.Pending.Name(),
                CreatedAt = order.CreatedAt,
                CreatedBy = order.CreatedBy
            };

            await paymentRepository.InsertAsync(payment);

            var paymentInformation = new PaymentInformationModel
            {
                Amount = request.Amount,
                OrderDescription = "Vaccine Payment",
                OrderType = "other",
                PaymentId = payment.PaymentId.ToString(),
                InjectionDate = formatInjectDate.ToString("yyyyMMddHHmmss"),
                ChildId = request.ChildId.ToString(),
                OrderInfo =  $"{payment.PaymentId.ToString()}, {request.ChildId}, {formatInjectDate.ToString("yyyyMMddHHmmss")}"
            };

            // string url = _vnPayService.CreatePaymentUrl(paymentInformation, httpContext);
            string url = "";
            switch (request.PaymentMethod)
            {
                case var paymentMethod when string.Equals(StaticEnum.PaymentMethodEnum.VnPay.Name(), paymentMethod, StringComparison.OrdinalIgnoreCase):
                {
                    payment.PaymentMethod = StaticEnum.PaymentMethodEnum.VnPay.Name();
                    url = _vnPayService.CreatePaymentUrl(paymentInformation, httpContext);
                    break;
                }
                case var paymentMethod when string.Equals(StaticEnum.PaymentMethodEnum.Momo.Name(), paymentMethod, StringComparison.OrdinalIgnoreCase):
                {
                    payment.PaymentMethod = StaticEnum.PaymentMethodEnum.Momo.Name();
                    MomoCreatePaymentResponseModel responseModel = await _momoService.CreatePaymentAsync(paymentInformation);
                    url = responseModel.PayUrl;
                    break;
                }
                default:
                    throw new CustomExceptions.EnumNotFoundException("Payment method enum not found");
            }
            
            await _unitOfWork.SaveChangeAsync();
            _unitOfWork.CommitTransaction();
            _logger.LogInformation("Done creating order async");
            _logger.LogInformation("Url payment: {}", url);
            return url;
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

    public async Task<bool> HandleVnPayResponse(IQueryCollection collections)
    {
        using var scope = _serviceProvider.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        try
        {
            var response = _vnPayService.PaymentExecute(collections);
            _logger.LogInformation("Start handle vn pay response with {}", response.ToString());
            
            var orderInfo = ExtractValues(response.OrderInfo);
            
            var paymentRepository = unitOfWork.GetRepository<Payment>();
            var orderRepository = unitOfWork.GetRepository<Order>();
            var listPayment = await paymentRepository.GetAllAsync(query => query
                .Include(p => p.User).Include(p => p.Order).ThenInclude(o => o.Child)
                .Where(p => p.PaymentId.ToString() == orderInfo.PaymentId));
            var payment = listPayment.FirstOrDefault();
            unitOfWork.BeginTransaction();

            payment.UpdatedAt = DateTime.Now;
            payment.UpdatedBy = StaticEnum.PaymentMethodEnum.VnPay.Name();

            if (!response.Success || response.VnTransactionStatus != "00")
            {
                var order = await orderRepository.FindAsync(o => o.OrderId == payment.OrderId);
                order.Status = StaticEnum.OrderStatusEnum.Cancelled.Name();
                await orderRepository.UpdateAsync(order);
                
                switch (response.VnPayResponseCode)
                {
                    case var responseCode when responseCode == StaticEnum.VnpResponseCode.Cancelled.Name():
                        payment.PaymentStatus = StaticEnum.PaymentStatusEnum.Cancelled.Name();
                        break;
                    default:
                        payment.PaymentStatus = StaticEnum.PaymentStatusEnum.Failed.Name();
                        break;
                }

                _logger.LogError("Error with vnpay paymnet " + response.VnTransactionStatus);
                await paymentRepository.UpdateAsync(payment);
                await unitOfWork.SaveChangeAsync();
                unitOfWork.CommitTransaction();
                return false;
            }

            await ChangeScheduleStatus(orderInfo.ChildId, orderInfo.InjectionDate, payment.OrderId.ToString(), unitOfWork);
            payment.PaymentStatus = StaticEnum.PaymentStatusEnum.Paid.Name();

            await paymentRepository.UpdateAsync(payment);
            await unitOfWork.SaveChangeAsync();
            unitOfWork.CommitTransaction();

            await SendEmail(payment);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("Error at handle vn pay response {}", e.Message);
            unitOfWork.RollBack();
            return false;
        }

    }

    public async Task<bool> HandleMomoResponse(IQueryCollection collection)
    {
        using var scope = _serviceProvider.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        try
        {
            var response = _momoService.GetFullResponseData(collection);
            var orderInfo = ExtractValues(response.OrderInfo);

            var paymentRepository = unitOfWork.GetRepository<Payment>();
            var orderRepository = unitOfWork.GetRepository<Order>();

            var listPayment = await paymentRepository.GetAllAsync(query => query
                .Include(p => p.User).Include(p => p.Order).ThenInclude(o => o.Child)
                .Where(p => p.PaymentId.ToString() == orderInfo.PaymentId));
            var payment = listPayment.FirstOrDefault();
            unitOfWork.BeginTransaction();

            payment.UpdatedAt = DateTime.Now;
            payment.UpdatedBy = StaticEnum.PaymentMethodEnum.Momo.Name();

            if (!response.Success || response.ErrorCode != "0")
            {
                var order = await orderRepository.FindAsync(o => o.OrderId == payment.OrderId);
                order.Status = StaticEnum.OrderStatusEnum.Cancelled.Name();
                await orderRepository.UpdateAsync(order);

                switch (response.ErrorCode)
                {
                    case var responseCode when responseCode == StaticEnum.MomoResponseCode.Failed.Name():
                        payment.PaymentStatus = StaticEnum.PaymentStatusEnum.Cancelled.Name();
                        break;
                    case var responseCode when responseCode == StaticEnum.MomoResponseCode.Rejected.Name():
                        payment.PaymentStatus = StaticEnum.PaymentStatusEnum.Rejected.Name();
                        break;
                    default:
                        payment.PaymentStatus = StaticEnum.PaymentStatusEnum.Failed.Name();
                        break;
                }

                _logger.LogError($"Error with momo paymnet with error code {response.ErrorCode} and message {response.ErrorMessage}");
                await paymentRepository.UpdateAsync(payment);
                await unitOfWork.SaveChangeAsync();
                unitOfWork.CommitTransaction();
                return false;
            }
            
            await ChangeScheduleStatus(orderInfo.ChildId, orderInfo.InjectionDate, payment.OrderId.ToString(), unitOfWork);
            payment.PaymentStatus = StaticEnum.PaymentStatusEnum.Paid.Name();

            await paymentRepository.UpdateAsync(payment);
            await unitOfWork.SaveChangeAsync();
            unitOfWork.CommitTransaction();

            await SendEmail(payment);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("Error at handle momo response {}", e.Message);
            unitOfWork.RollBack();
            return false;
        }

    }

    private async Task ChangeScheduleStatus(string childId, string injectionDate, string orderId, IUnitOfWork unitOfWork)
    {
        try
        {
            _logger.LogInformation("Start retrieving schedule from redis");
            var scheduleRepository = unitOfWork.GetRepository<Schedule>();

            string key = $"schedule:{childId}:{injectionDate}";
            var listSchedule = await _cacheService.GetAsync<List<Schedule>>(key);

            if (listSchedule.IsNullOrEmpty()) throw new KeyNotFoundException("List Schedule not found in the redis");

            listSchedule.ForEach(s =>
            {
                s.status = StaticEnum.ScheduleStatusEnum.Upcoming.Name();
                s.OrderId = Guid.Parse(orderId);
                s.ScheduleDate = s.ScheduleDate?.ToUniversalTime();
                s.ActualDate = s.ActualDate?.ToUniversalTime();
                s.CreatedAt = s.CreatedAt?.ToUniversalTime();
                s.UpdatedAt = s.UpdatedAt?.ToUniversalTime();
            });       
            await scheduleRepository.InsertRangeAsync(listSchedule);
            _logger.LogInformation("Schedule status updated");
        }
        catch (Exception e)
        {
            _logger.LogError("Error when save schedule redis cause by {}", e.Message);
            throw;
        }
    }

    private (string PaymentId, string ChildId, string InjectionDate) ExtractValues(string input)
    {
        string[] values = input.Split(", ");

        if (values.Length == 3)
        {
            return (values[0], values[1], values[2]);
        }

        throw new ArgumentException("Invalid input format.");
    }
    
    private async Task SendEmail(Payment payment)
    {
        try
        {
            var notificationRepository = _unitOfWork.GetRepository<Notification>();
            Notification notification = new Notification()
            {
                UserId = payment.UserId,
                TemplateId = StaticEnum.EmailTemplateEnum.AppointmentConfirmation.Id(),
                ChildId = payment.Order.ChildId,
                IsRead = false,
                CreatedAt = DateTime.Now,
                CreatedBy = "System"
            };
            _logger.LogInformation("Start sending email to customer {}", payment.User.Email);
            var param = new Dictionary<string, string>()
            {
                { "Customer Name", payment.User.UserName  },
                { "Appointment Date", payment.Order.OrderDate?.ToString("dd-MM-yyyy") ?? "N/A"},
                { "Children Name", payment.Order.Child.FullName},
                { "Phone Number", payment.User.Phone},
                {"Payment ID", payment.PaymentId.ToString()},
                {"Payment Amount", payment.Amount.ToString()},
                {"Payment Method", payment.PaymentMethod}
            };
            await _emailService.SendEmailAsync(payment.User.Email, payment.User.FullName,
                StaticEnum.EmailTemplateEnum.AppointmentConfirmation.Id(), param);

            await notificationRepository.InsertAsync(notification);
            await _unitOfWork.SaveChangeAsync();
            _logger.LogInformation("Successfully send email to {}", payment.User.Email);
        }
        catch (Exception e)
        {
            _logger.LogError("Error at send mail after payment cause by {}", e.Message);
            throw;
        }
    }
    
    
    
}