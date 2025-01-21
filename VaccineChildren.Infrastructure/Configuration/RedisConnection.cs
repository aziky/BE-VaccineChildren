namespace VaccineChildren.Infrastructure.Configuration;

public class RedisConnection
{
    public string Host { get; set; }
    public int Port { get; set; }
    public int Timeout { get; set; }

    public string GetConnectionString()
    {
        return $"{Host}:{Port},connectTimeout={Timeout}";
    }

}