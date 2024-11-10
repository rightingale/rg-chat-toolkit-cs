using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;

namespace epic_retail_api_cs.Cache;

public class RedisCacheService : CacheService
{
    // Redis connection:
    protected readonly ConnectionMultiplexer redis;

    protected readonly string prefix;
    protected readonly TimeSpan expiration;

    protected readonly string redisHost;
    protected readonly int redisPort;
    protected readonly string redisDatabase;
    protected readonly string redisPassword;

    public RedisCacheService(TimeSpan expiration, string prefix, string redisHost, int redisPort, string redisDatabase, string redisPassword)
    {
        this.redisHost = redisHost;
        this.redisPort = redisPort;
        this.redisDatabase = redisDatabase;
        this.redisPassword = redisPassword;

        this.prefix = prefix;
        this.expiration = expiration;

        var configurationOptions = new ConfigurationOptions
        {
            EndPoints = { $"{redisHost}:{redisPort}" },
            Password = redisPassword,
            ServiceName = redisDatabase
        };
        this.redis = ConnectionMultiplexer.Connect(configurationOptions);
    }

    public override async Task<T> GetOrCreate<T>(object key, Func<Task<T>> factory)
    {
        key = prefix + "-" + key;

        IDatabase db = this.redis.GetDatabase();
        var cachedJson = await db.StringGetAsync(key.ToString());

#if DEBUG
        if (cachedJson.HasValue)
        {
            Console.WriteLine("### Cache HIT!! [" + key + "]");
            Console.WriteLine($"The cached value of '{key}' is: {cachedJson}");
        }
        else
        {
            Console.WriteLine("### Cache miss [" + key + "]");
        }
#endif

        if (cachedJson.HasValue)
        {
            // Deserialize cached json:
            T cachedValueDeserialized = JsonConvert.DeserializeObject<T>(cachedJson);
            return cachedValueDeserialized;
        }
        else
        {
            // Populate:
            T computedValue = await factory.Invoke();

            // Persist to cache:
            string computedValueJson = JsonConvert.SerializeObject(computedValue);
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            db.StringSetAsync(key.ToString(), computedValueJson, this.expiration, false);

            return computedValue;
        }
    }

    public async Task<bool> Put(string key, string value)
    {
        key = prefix + "-" + key;

        IDatabase db = this.redis.GetDatabase();
        return await db.StringSetAsync(key, value, this.expiration, false);
    }

    public async Task<string> Get(string key)
    {
        key = prefix + "-" + key;

        IDatabase db = this.redis.GetDatabase();
        return await db.StringGetAsync(key);
    }

    public async Task Remove(string key)
    {
        key = prefix + "-" + key;

        IDatabase db = this.redis.GetDatabase();

        // Find keys matching pattern:
        var keys = this.redis.GetServer(this.redis.GetEndPoints()[0]).Keys(pattern: key);
        List<Task<bool>> deleteTasks = new List<Task<bool>>();
        foreach (var k in keys)
        {
            // Make a list of async handlers and wait for them all to complete
            var task = db.KeyDeleteAsync(k);
            deleteTasks.Add(task);
        }

        await Task.WhenAll(deleteTasks);

        return;
    }
}
