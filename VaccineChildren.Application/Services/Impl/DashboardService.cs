using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VaccineChildren.Application.DTOs.Response;
using VaccineChildren.Core.Exceptions;
using VaccineChildren.Core.Store;
using VaccineChildren.Domain.Abstraction;
using VaccineChildren.Domain.Entities;

namespace VaccineChildren.Application.Services.Impl;

public class DashboardService : IDashboardService
{
    private readonly ILogger<DashboardService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public DashboardService(ILogger<DashboardService> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }


    public async Task<AccountRes> GetAccountAsync(int year)
    {
        try
        {
            _logger.LogInformation("{ClassName} - Getting account from dashboard", nameof(DashboardService));
            var userRepository = _unitOfWork.GetRepository<User>();
            var staffRepository = _unitOfWork.GetRepository<Staff>();
            int totalAccount = 0;
            AccountRes accountRes = new AccountRes();

            IList<Staff> staffList = await staffRepository.GetAllAsync(query => query.Include(r => r.Role)
                .Where(s => s.CreatedAt.HasValue && s.CreatedAt.Value.Year <= year));

            foreach (var staff in staffList)
            {
                if (staff.Role == null) continue;

                switch (staff.Role.RoleName)
                {
                    case var roleName when roleName == StaticEnum.RoleEnum.Admin.Name():
                        UpdateAccountCount(accountRes, staff,
                            StaticEnum.AccountEnum.AdminWorking,
                            StaticEnum.AccountEnum.AdminResigned, ref totalAccount);
                        break;

                    case var roleName when roleName == StaticEnum.RoleEnum.Manager.Name():
                        UpdateAccountCount(accountRes, staff,
                            StaticEnum.AccountEnum.ManagerWorking,
                            StaticEnum.AccountEnum.ManagerResigned, ref totalAccount);
                        break;

                    case var roleName when roleName == StaticEnum.RoleEnum.Staff.Name():
                        UpdateAccountCount(accountRes, staff,
                            StaticEnum.AccountEnum.StaffWorking,
                            StaticEnum.AccountEnum.StaffResigned, ref totalAccount);
                        break;
                }
            }

            IList<User> userList = await userRepository.GetAllAsync(query => query.Include(r => r.Role)
                .Where(u => u.CreatedAt.HasValue && u.CreatedAt.Value.Year <= year &&
                            u.Role.RoleName.ToLower() == StaticEnum.RoleEnum.User.Name()));
            int userCount = userList.Count;
            if (userCount == 0 || totalAccount == 0)
            {
                throw new CustomExceptions.NoDataFoundException("There's no account");
            }

            accountRes.AccountDictionary[StaticEnum.AccountEnum.UserAccount.Name()] = userCount;
            accountRes.AccountDictionary["totalAccount"] = totalAccount + userCount;
            _logger.LogInformation("Getting account from dashboard done");
            return accountRes;
        }
        catch (Exception e)
        {
            _logger.LogError($"{nameof(DashboardService)} - Error at get account async cause by: {e.Message}");
            throw;
        }
    }

    public async Task<RevenueDataRes> GetRevenueDataAsync(int year)
    {
        try
        {
            _logger.LogInformation($"{nameof(DashboardService)} - Getting revenue data from dashboard");
            var paymentRepository = _unitOfWork.GetRepository<Payment>();
            IList<Payment> paymentList = await paymentRepository.GetAllAsync(query => query.Where(p =>
                p.CreatedAt.Value.Year == year
                && p.PaymentStatus.ToLower() == StaticEnum.PaymentStatusEnum.Completed.Name().ToLower())
            );

            if (paymentList.Count == 0)
            {
                throw new CustomExceptions.NoDataFoundException("There's data for the payment");
            }

            List<RevenueDataRes.MonthlyRevenue> monthlyRevenuesList = paymentList
                    .Where(p => p.PaymentDate.HasValue)
                    .GroupBy(p => p.PaymentDate.Value.Month)
                    .Select(group => new RevenueDataRes.MonthlyRevenue
                    {
                        Month = group.Key,
                        Amount = Convert.ToDouble(group.Sum(p => p.Amount ?? 0))
                    })
                    .OrderBy(m => m.Month)
                    .ToList()
                ;
            _logger.LogInformation($"{nameof(DashboardService)} - Done getting revenue data from dashboard");
            return new RevenueDataRes
            {
                Year = year,
                TotalRevenue = Convert.ToDouble(monthlyRevenuesList.Sum(m => m.Amount)),
                MonthlyRevenueList = monthlyRevenuesList
            };
        }
        catch (Exception e)
        {
            _logger.LogError(
                $"{nameof(DashboardService)} - Error at get revenue data by month in year {year} async cause by: {e.Message}");
            throw;
        }
    }

    public async Task<DashboardSummaryRes> GetDashboardSummaryAsync(int year)
    {
        try
        {
            _logger.LogInformation($"{nameof(DashboardService)} - Getting dashboard summary");

            var vaccineRepository = _unitOfWork.GetRepository<Vaccine>();
            var scheduleRepository = _unitOfWork.GetRepository<Schedule>();
            var packageRepository = _unitOfWork.GetRepository<Package>();
            var manufacturerRepository = _unitOfWork.GetRepository<Manufacturer>();
            var orderRepository = _unitOfWork.GetRepository<Order>();

            var packageAvailableList = await packageRepository.GetAllAsync(query => query
                .Where(p => p.IsActive == true && p.CreatedAt.Value.Year == year));
            var vaccineActiveList = await vaccineRepository.GetAllAsync(query => query
                .Where(v => v.IsActive == true && v.CreatedAt.Value.Year == year));
            var vaccinatedCustomerList = await scheduleRepository.GetAllAsync(query => query
                .Include(c => c.Child)
                .Where(s => s.IsVaccinated == true && s.CreatedAt.Value.Year == year));
            var manufacturerList = await manufacturerRepository.GetAllAsync(query => query
                .Include(m => m.VaccineManufactures).ThenInclude(vm => vm.Batches.Where(b => b.IsActive == true))
                .Where(m => m.IsActive == true && m.CreatedAt.Value.Year == year)
            );

            var vaccineOrderedList = await orderRepository.GetAllAsync(query => query
                .Include(o => o.Vaccines)
                .Include(o => o.Packages).ThenInclude(p => p.Services)
            );
            
            _logger.LogInformation($"{nameof(DashboardService)} - Get data successfully from dashboard summary");

            List<VaccineData> vaccineData = vaccineOrderedList
                .SelectMany(o =>
                    o.Vaccines
                        .Select(vm => vm.Vaccine)
                        .Concat(o.Packages.SelectMany(p => p.Services)) 
                        .Select(v => new VaccineData.VaccineDetails
                        {
                            VaccineId = v.VaccineId.ToString(),
                            VaccineName = v.VaccineName
                        })
                )
                .GroupBy(v => vaccineOrderedList.Count(o =>
                    o.Vaccines.Any(vm => vm.Vaccine.VaccineId.ToString() == v.VaccineId) || 
                    o.Packages.Any(p => p.Services.Any(s => s.VaccineId.ToString() == v.VaccineId))
                ))
                .Where(g => g.Key > 0)
                .Select(g => new VaccineData
                {
                    NumberVaccinated = g.Key,
                    ListVaccine = g.DistinctBy(v => v.VaccineId).ToList()
                })
                .OrderByDescending(vd => vd.NumberVaccinated)
                .ThenBy(vd => vd.ListVaccine.FirstOrDefault()?.VaccineName)
                .ToList();
        

            List<ManufacturerData> manufacturerData = manufacturerList
                .Select(m => new ManufacturerData
                {
                    ManufacturerId = m.ManufacturerId.ToString(),
                    ManufacturerName = m.Name,
                    ManufacturerShortName = m.ShortName,
                    NumberBatch = m.VaccineManufactures
                        .SelectMany(vm => vm.Batches)
                        .Sum(b => b.Quantity ?? 0)
                })
                .OrderByDescending(m => m.NumberBatch).Take(5).ToList();

            List<AgeData> ageData = vaccinatedCustomerList
                .DistinctBy(s => s.ChildId)
                .Select(s => new
                {
                    Age = DateTime.Now.Year - s.Child.Dob.Value.Year - 
                          (DateTime.Now.DayOfYear < s.Child.Dob.Value.DayOfYear ? 1 : 0)
                })
                .GroupBy(ag => ag.Age)
                .Select(g => new AgeData
                {
                    Age = g.Key,
                    NumberVaccinated = g.Count()
                })
                .OrderByDescending(a => a.Age).Take(5).ToList();
            
            _logger.LogInformation($"{nameof(DashboardService)} - Done getting dashboard summary");

            return new DashboardSummaryRes
            {
                Year = year,
                TotalAvailableVaccines = vaccineActiveList.Count,
                TotalVaccinatedCustomers = vaccinatedCustomerList.Count,
                TotalAvailablePackages = packageAvailableList.Count,
                AgeData = ageData,
                ManufacturerData = manufacturerData,
                VaccineData = vaccineData.Count > 5 ? vaccineData.Take(5).ToList() : vaccineData
            };
        }
        catch (Exception e)
        {
            _logger.LogError($"{nameof(DashboardService)} - Error at get dashboard summary async cause by: {e.Message}");
            throw;
        }
    }

    private void UpdateAccountCount(AccountRes accountRes, Staff staff, StaticEnum.AccountEnum workingEnum,
        StaticEnum.AccountEnum resignedEnum, ref int totalAccount)
    {
        var isActive = string.Equals(StaticEnum.StatusEnum.Active.Name(), staff.Status,
            StringComparison.OrdinalIgnoreCase);
        var key = isActive ? workingEnum.Name() : resignedEnum.Name();

        accountRes.AccountDictionary[key] = accountRes.AccountDictionary.TryGetValue(key, out int currentValue)
            ? currentValue + 1
            : 1;
        totalAccount += 1;
    }
}