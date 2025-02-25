using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration; 
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using VaccineChildren.Domain.Abstraction;
using VaccineChildren.Infrastructure.Configuration;
using VaccineChildren.Infrastructure.Implementation;
using VaccineChildren.Application.Services;
using VaccineChildren.Application.Services.Impl;

namespace VaccineChildren.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddLogging();
        var databaseSettings = new DatabaseConnection(
            services.BuildServiceProvider().GetRequiredService<ILogger<DatabaseConnection>>()
        );
        configuration.GetSection("DatabaseConnection").Bind(databaseSettings);
        services.AddSingleton(databaseSettings);
        services.AddDatabase(databaseSettings);

        // var redisSettings = new RedisConnection();
        var redisSettings = new RedisConnection(     
            services.BuildServiceProvider().GetRequiredService<ILogger<RedisConnection>>()
        );
        configuration.GetSection("RedisConnection").Bind(redisSettings);
        services.AddSingleton(redisSettings);
        services.AddRedis(redisSettings);

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICacheService, RedisCacheService>();
        services.AddScoped<IRsaService, RsaService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddHttpClient<IProvinceService, ProvinceService>();
        services.AddScoped<IVnPayService, VnPayService>();

    }

    private static void AddDatabase(this IServiceCollection services, DatabaseConnection databaseSettings)
    {
        services.AddDbContext<VaccineSystemDbContext>(options =>
        {
            options.UseLazyLoadingProxies()
                .UseNpgsql(databaseSettings.ToConnectionString());
        });
    }

    private static void AddRedis(this IServiceCollection services, RedisConnection redisSettings)
    {
        // Thay thế AddStackExchangeRedisCache bằng đăng ký trực tiếp ConnectionMultiplexer
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var configuration = ConfigurationOptions.Parse(redisSettings.GetConnectionString());
            return ConnectionMultiplexer.Connect(configuration);
        });
    }
}