namespace VaccineChildren.API;

public static class DependencyInjection
{
    public static void AddConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RouteOptions>(options => { options.LowercaseUrls = true; });
        ConfigApplication(services);
        ConfigInfrastructure(services, configuration);
        services.AddLogging();
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