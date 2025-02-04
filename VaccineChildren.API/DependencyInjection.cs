namespace VaccineChildren.API;

public static class DependencyInjection
{
    public static void AddConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RouteOptions>(options => { options.LowercaseUrls = true; });
        ConfigApplication(services);
        ConfigInfrastructure(services, configuration);
        services.AddLogging();
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.WithOrigins("http://localhost:5173/", "http://localhost:5173/")
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
    }

    private static void ConfigApplication(IServiceCollection services)
    {
        Application.DependencyInjection.AddApplication(services);
    }

    private static void ConfigInfrastructure(IServiceCollection services, IConfiguration configuration)
    {
        Infrastructure.DependencyInjection.AddInfrastructure(services, configuration);
    }
}