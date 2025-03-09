using Microsoft.Extensions.DependencyInjection;
using VaccineChildren.Application.Services;
using VaccineChildren.Application.Services.Impl;
using VaccineChildren.Domain.Abstraction;

namespace VaccineChildren.Application;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile));
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IStaffService, StaffService>();
        services.AddScoped<IVaccineService, VaccineService>();
        services.AddScoped<IManufacturerService, ManufacturerService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IPackageService, PackageService>();
        services.AddScoped<IStaffScheduleService, StaffScheduleService>();
        services.AddScoped<IBatchService, BatchService>();
        services.AddScoped<IVaccineCheckupService, VaccineCheckupService>();
        services.AddScoped<IScheduleService, ScheduleService>();
        services.AddHttpClient<IGoogleAuthService, GoogleAuthService>();
    }
}