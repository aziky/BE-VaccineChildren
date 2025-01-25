using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VaccineChildren.Application.DTOs.Response;
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


    public async Task<AccountRes> GetAccountAsync()
    {
        try
        {
            _logger.LogInformation("{ClassName} - Getting account from dashboard", nameof(DashboardService));
            var userRepository = _unitOfWork.GetRepository<User>();
            var staffRepository = _unitOfWork.GetRepository<Staff>();
            AccountRes accountRes = new AccountRes();

            IList<Staff> staffList = await staffRepository.GetAllAsync(query => query.Include(r => r.Role));

            foreach (var staff in staffList)
            {
                if (staff.Role == null) continue;

                switch (staff.Role.RoleName)
                {
                    case var roleName when roleName == StaticEnum.RoleEnum.Admin.Name():
                        UpdateAccountCount(accountRes, staff,
                            StaticEnum.AccountEnum.AdminWorking,
                            StaticEnum.AccountEnum.AdminResigned);
                        break;

                    case var roleName when roleName == StaticEnum.RoleEnum.Manager.Name():
                        UpdateAccountCount(accountRes, staff,
                            StaticEnum.AccountEnum.ManagerWorking,
                            StaticEnum.AccountEnum.ManagerResigned);
                        break;

                    case var roleName when roleName == StaticEnum.RoleEnum.Staff.Name():
                        UpdateAccountCount(accountRes, staff,
                            StaticEnum.AccountEnum.StaffWorking,
                            StaticEnum.AccountEnum.StaffResigned);
                        break;
                }
            }

            IList<User> userList = await userRepository.GetAllAsync(query => query.Include(r => r.Role));
            accountRes.AccountDictionary[StaticEnum.AccountEnum.UserAccount.Name()] = userList.Count;

            _logger.LogInformation("Getting account from dashboard done");
            return accountRes;
        }
        catch (Exception e)
        {
            _logger.LogError("{ClassName} - Error at get account async cause by: {error}", nameof(DashboardService),
                e.Message);
            throw;
        }
    }

    private void UpdateAccountCount(AccountRes accountRes, Staff staff, StaticEnum.AccountEnum workingEnum, 
        StaticEnum.AccountEnum resignedEnum)
    {
        var isActive = string.Equals(StaticEnum.StatusEnum.Active.Name(), staff.Status, StringComparison.OrdinalIgnoreCase);
        var key = isActive ? workingEnum.Name() : resignedEnum.Name();

        accountRes.AccountDictionary[key] = accountRes.AccountDictionary.TryGetValue(key, out int currentValue) 
                ? currentValue + 1 : 1;
    }
}