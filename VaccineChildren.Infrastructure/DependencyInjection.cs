using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VaccineChildren.Domain.Abstraction;
using VaccineChildren.Infrastructure.Configuration;
using VaccineChildren.Infrastructure.Implementation;

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

        var redisSettings = new RedisConnection();
        configuration.GetSection("RedisConnection").Bind(redisSettings);
        services.AddSingleton(redisSettings);
        services.AddRedis(redisSettings);

        services.AddScoped<IUnitOfWork, UnitOfWork>();
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
        services.AddStackExchangeRedisCache(options => options.Configuration = redisSettings.GetConnectionString());
    }
}