using StackExchange.Redis;
using System.Text.Json;

namespace eCommerce.OrdersService.Infrastructure.Cache;

public class RedisCacheService : ICacheService
{
    private readonly IDatabase _db;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public RedisCacheService(IConnectionMultiplexer connection)
    {
        _db = connection.GetDatabase();
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        var data = await _db.StringGetAsync(key);
        if (data.IsNullOrEmpty)
            return default;

        return await SafeDeserialize<T>(data!, key);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? ttl = null, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(value, JsonOptions);
        await _db.StringSetAsync(key, json, ttl);
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default)
    {
        await _db.KeyDeleteAsync(key);
    }

    public async Task<IDictionary<string, T?>> GetManyAsync<T>(IEnumerable<string> keys, CancellationToken ct = default)
    {
        var redisKeys = keys.Select(k => new RedisKey(k)).ToArray();
        var jsonData = await _db.StringGetAsync(redisKeys);

        var dict = new Dictionary<string, T?>();
        int i = 0;

        foreach (var key in keys) 
        {
            if (jsonData[i].IsNullOrEmpty)
                dict[key] = default;
            else
                dict[key] = await SafeDeserialize<T>(jsonData[i]!, key);

            i++;
        }

        return dict;
    }

    public async Task SetManyAsync<T>(IDictionary<string, T> values, TimeSpan? ttl = null, CancellationToken ct = default)
    {
        var kvps = values
            .Select(kvp => new KeyValuePair<RedisKey, RedisValue>(
                kvp.Key, 
                JsonSerializer.Serialize(kvp.Value, JsonOptions)))
            .ToArray();

        await _db.StringSetAsync(kvps);

        if (ttl is not null)
        {
            var tasks = kvps.Select(e => _db.KeyExpireAsync(e.Key, ttl));
            await Task.WhenAll(tasks);
        }
    }

    private async Task<T?> SafeDeserialize<T>(string json, string key)
    {
        try
        {
            return JsonSerializer.Deserialize<T>(json, JsonOptions);
        }
        catch (JsonException)
        {
            await _db.KeyDeleteAsync(key);
            return default;
        }
    }
}
