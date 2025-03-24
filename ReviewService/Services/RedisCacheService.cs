using System;
using System.Text.Json;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace ReviewService.Services
{
    public interface ICacheService<T>
    {
        Task<T> GetOrSetAsync(string key, Func<Task<T>> factory, TimeSpan? expiration = null);
        Task RemoveAsync(string key);
        Task RemoveByPatternAsync(string pattern);
    }

    public class RedisCacheService<T> : ICacheService<T>
    {
        private readonly IDatabase _cache;
        private readonly TimeSpan _defaultExpiration;

        public RedisCacheService(IConnectionMultiplexer redis, TimeSpan? defaultExpiration = null)
        {
            _cache = redis.GetDatabase();
            _defaultExpiration = defaultExpiration ?? TimeSpan.FromMinutes(10);
        }

        public async Task<T> GetOrSetAsync(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
        {
            var cachedValue = await _cache.StringGetAsync(key);
            if (!cachedValue.IsNullOrEmpty)
            {
                return JsonSerializer.Deserialize<T>(cachedValue);
            }

            var value = await factory();
            var serializedValue = JsonSerializer.Serialize(value);
            await _cache.StringSetAsync(key, serializedValue, expiration ?? _defaultExpiration);
            return value;
        }

        public async Task RemoveAsync(string key)
        {
            await _cache.KeyDeleteAsync(key);
        }

        public async Task RemoveByPatternAsync(string pattern)
        {
            var endpoints = _cache.Multiplexer.GetEndPoints();
            var server = _cache.Multiplexer.GetServer(endpoints[0]);
            var keys = server.Keys(pattern: pattern);
            
            foreach (var key in keys)
            {
                await _cache.KeyDeleteAsync(key);
            }
        }
    }
} 