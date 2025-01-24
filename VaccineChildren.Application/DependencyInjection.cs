using Microsoft.Extensions.DependencyInjection;
using VaccineChildren.Application.DTOs;
using VaccineChildren.Application.Services;
using VaccineChildren.Application.Services.Impl;

namespace VaccineChildren.Application;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile));
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IStaffService, StaffService>();
    }
}