using Microsoft.Extensions.Logging;

namespace VaccineChildren.Infrastructure.Configuration;

public class RedisConnection
{
    
    private readonly ILogger<RedisConnection> _logger;

    public RedisConnection()
    {
        
    }
    
    public RedisConnection(ILogger<RedisConnection> logger)
    {
        _logger = logger;
    }
    
    public string Host { get; set; }
    public int Port { get; set; }
    public int Timeout { get; set; }

    public string GetConnectionString()
    {
        _logger.LogInformation($"Redis connection: {Host}:{Port},connectTimeout={Timeout}");
        return $"{Host}:{Port},connectTimeout={Timeout}";
    }

}