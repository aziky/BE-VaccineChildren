using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
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
            AccountRes accountRes = new AccountRes();

            IList<User> userList = await userRepository.GetAllAsync(query => query.AsNoTracking()
                .Include(r => r.Role).Include(u => u.Staff)
                .Where(s => s.CreatedAt.HasValue && s.CreatedAt.Value.Year <= year));

            foreach (var user in userList)
            {
                if (user.Role.RoleName.Equals(StaticEnum.RoleEnum.User.Name())) continue;

                switch (user.Role.RoleName)
                {
                    case var roleName when roleName == StaticEnum.RoleEnum.Admin.Name():
                        UpdateAccountCount(accountRes, user.Staff,
                            StaticEnum.AccountEnum.AdminWorking,
                            StaticEnum.AccountEnum.AdminResigned);
                        break;

                    case var roleName when roleName == StaticEnum.RoleEnum.Manager.Name():
                        UpdateAccountCount(accountRes, user.Staff,
                            StaticEnum.AccountEnum.ManagerWorking,
                            StaticEnum.AccountEnum.ManagerResigned);
                        break;

                    case var roleName when roleName == StaticEnum.RoleEnum.Staff.Name():
                        UpdateAccountCount(accountRes, user.Staff,
                            StaticEnum.AccountEnum.StaffWorking,
                            StaticEnum.AccountEnum.StaffResigned);
                        break;
                    case var roleName when roleName == StaticEnum.RoleEnum.Doctor.Name(): 
                        UpdateAccountCount(accountRes, user.Staff,
                            StaticEnum.AccountEnum.DoctorWorking,
                            StaticEnum.AccountEnum.DoctorResigned);
                        break;
                    default:
                        throw new Exception("Unknow support role");
                }
            }

            if (userList.IsNullOrEmpty())
            {
                throw new CustomExceptions.NoDataFoundException("There's no account");
            }

            accountRes.AccountDictionary[StaticEnum.AccountEnum.UserAccount.Name()] = userList.Count(u => u.Role.RoleName == StaticEnum.RoleEnum.User.Name());
            accountRes.AccountDictionary["totalAccount"] =  userList.Count;
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
                && p.PaymentStatus.ToLower() == StaticEnum.PaymentStatusEnum.Paid.Name().ToLower())
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

            var packageAvailableList = await packageRepository.GetAllAsync(query => query
                .Where(p => p.IsActive == true && p.CreatedAt.Value.Year == year));
            var vaccineActiveList = await vaccineRepository.GetAllAsync(query => query
                .Where(v => v.IsActive == true && v.CreatedAt.Value.Year == year));
            var manufacturerList = await manufacturerRepository.GetAllAsync(query => query
                .Include(m => m.VaccineManufactures).ThenInclude(vm => vm.Batches.Where(b => b.IsActive == true))
                .Where(m => m.IsActive == true && m.CreatedAt.Value.Year == year)
            );
            
            _logger.LogInformation($"{nameof(DashboardService)} - Get data successfully from dashboard summary");
            
            IList<Schedule> scheduleList = await scheduleRepository.GetAllAsync(query => query
                .Include(s => s.Vaccine)
                .Where(s => s.status.Equals(StaticEnum.ScheduleStatusEnum.Completed.Name()))
            );
            List<VaccineData> top5Vaccine = MapToTop5VaccineData(scheduleList);
            
            List<ManufacturerData> top5Manufacture = MapToTop5ManufacturerData(manufacturerList);

            List<AgeData> top5AgeData = MapTo5AgeData(scheduleList);
                
            
            _logger.LogInformation($"{nameof(DashboardService)} - Done getting dashboard summary");

            return new DashboardSummaryRes
            {
                Year = year,
                TotalAvailableVaccines = vaccineActiveList.Count,
                TotalVaccinatedCustomers = scheduleList.DistinctBy(s => s.ChildId).Count(),
                TotalAvailablePackages = packageAvailableList.Count,
                AgeData = top5AgeData,
                ManufacturerData = top5Manufacture,
                VaccineData = top5Vaccine
            };
        }
        catch (Exception e)
        {
            _logger.LogError($"{nameof(DashboardService)} - Error at get dashboard summary async cause by: {e.Message}");
            throw;
        }
    }
    
    
    private List<VaccineData> MapToTop5VaccineData(IList<Schedule> schedules)
    {
        try
        {
            _logger.LogInformation("Star mapping top 5 vaccinated data");
            return schedules
                .GroupBy(s => s.Vaccine.VaccineId)
                .Select(g => new VaccineData
                {
                    NumberVaccinated = g.Count(),
                    ListVaccine = new List<VaccineData.VaccineDetails>
                    {
                        new VaccineData.VaccineDetails
                        {
                            VaccineId = g.Key.ToString(),
                            VaccineName = g.First().Vaccine.VaccineName
                        }
                    }
                })
                .OrderByDescending(v => v.NumberVaccinated)
                .Take(5)
                .ToList();
        }
        catch (Exception e)
        {
            _logger.LogError("Error at mapping top 5 vaccine data {}",e.Message);
            throw new Exception("Error at mapping top 5 vaccine data");
        }
    }
    
    private List<ManufacturerData> MapToTop5ManufacturerData(IList<Manufacturer> manufacturerList)
    {
        try
        {
            _logger.LogInformation("Star mapping top 5 manufacturers");
            return manufacturerList.Select(m => new ManufacturerData
                {
                    ManufacturerId = m.ManufacturerId.ToString(),
                    ManufacturerName = m.Name,
                    ManufacturerShortName = m.ShortName,
                    NumberBatch = m.VaccineManufactures
                        .SelectMany(vm => vm.Batches)
                        .Sum(b => b.Quantity ?? 0)
                })
                .OrderByDescending(m => m.NumberBatch)
                .Take(5).ToList();
        }
        catch (Exception e)
        {
            _logger.LogError("Error at mapping top 5 manufacturer data cause by {}", e.Message);
            throw new Exception("Error at mapping top 5 manufacturer data "+ e.Message);
        }
    }


    private List<AgeData> MapTo5AgeData(IList<Schedule> schedules)
    {
        try
        {
            _logger.LogInformation("Star mapping top 5 age data");
            return schedules.DistinctBy(s => s.ChildId)
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
        }
        catch (Exception e)
        {
            _logger.LogError("Error at mapping 5 age data cause by {}", e.Message);
            throw new Exception("Error at mapping 5 age data");
        }
    }
    
    private void UpdateAccountCount(AccountRes accountRes, Staff staff, StaticEnum.AccountEnum workingEnum,
        StaticEnum.AccountEnum resignedEnum)
    {
        var isActive = string.Equals(StaticEnum.StatusEnum.Active.Name(), staff.Status,
            StringComparison.OrdinalIgnoreCase);
        var key = isActive ? workingEnum.Name() : resignedEnum.Name();

        accountRes.AccountDictionary[key] = accountRes.AccountDictionary.TryGetValue(key, out int currentValue)
            ? currentValue + 1
            : 1;
    }
}