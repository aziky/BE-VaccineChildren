using StackExchange.Redis;
using System.Text.Json;
using System.Threading.Tasks;

namespace VaccineChildren.Application.Services // Điều chỉnh namespace theo dự án của bạn
{
    public class RedisCacheService : ICacheService
    {
        private readonly IConnectionMultiplexer _redisConnection;
        private readonly IDatabase _cache;

        public RedisCacheService(IConnectionMultiplexer redisConnection)
        {
            _redisConnection = redisConnection ?? throw new ArgumentNullException(nameof(redisConnection));
            _cache = redisConnection.GetDatabase();
        }
        
        public async Task<T?> GetAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key), "Cache key cannot be null or empty.");

            var value = await _cache.StringGetAsync(key);
            if (value.IsNullOrEmpty)
                return default;

            try
            {
                return JsonSerializer.Deserialize<T>(value!);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Failed to deserialize cached value for key '{key}'.", ex);
            }
        }
        
        public async Task SetAsync<T>(string key, T value, TimeSpan? expirationTime = null)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key), "Cache key cannot be null or empty.");

            if (value == null)
                throw new ArgumentNullException(nameof(value), "Cache value cannot be null.");

            try
            {
                var serializedValue = JsonSerializer.Serialize(value);
                await _cache.StringSetAsync(key, serializedValue, expirationTime ?? TimeSpan.FromHours(1));
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Failed to serialize value for key '{key}'.", ex);
            }
            catch (RedisException ex)
            {
                throw new InvalidOperationException($"Failed to set value in Redis for key '{key}'.", ex);
            }
        }
        
        public async Task RemoveAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key), "Cache key cannot be null or empty.");

            try
            {
                await _cache.KeyDeleteAsync(key);
            }
            catch (RedisException ex)
            {
                throw new InvalidOperationException($"Failed to remove key '{key}' from Redis.", ex);
            }
        }
    }
}