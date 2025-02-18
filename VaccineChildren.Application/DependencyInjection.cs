using Microsoft.Extensions.DependencyInjection;
using VaccineChildren.Application.Services;
using VaccineChildren.Application.Services.Impl;

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
        services.AddScoped<IVaccineScheduleService, VaccineScheduleService>();
        services.AddScoped<IBatchService, BatchService>();

    }
}