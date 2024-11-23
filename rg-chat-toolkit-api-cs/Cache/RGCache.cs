using Amazon.Runtime.Internal.Util;
using rg_chat_toolkit_cs.Cache;
using rg_chat_toolkit_api_cs.Chat;

namespace rg_chat_toolkit_api_cs.Cache;

public class RGCache
    : IRGCache, IRGMessageCache, IRGEmbeddingCache
{
    public static RGCache Instance = new RGCache();

    private static readonly IConfigurationRoot config = new ConfigurationManager()
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
        .AddUserSecrets<ChatCompletionController>()
        .AddEnvironmentVariables()
        .Build();

    protected static readonly RedisCacheService Cache;

    static RGCache()
    {
        string redisHost = config["redis:host"] ?? "";
        int redisPort = int.Parse(config["redis:port"] ?? "0");
        string redisDatabase = config["redis:database"] ?? "";
        string redisPassword = config["redis:password"] ?? "";

        RGCache.Cache = new RedisCacheService(
            expiration: CacheService.ABSOLUTE_EXPIRATION,
            prefix: "rg-",
            redisHost: redisHost,
            redisPort: redisPort,
            redisDatabase: redisDatabase,
            redisPassword: redisPassword
        );
    }

    public static string GetCacheKey(Guid tenantID, Guid sessionID, Guid accessKey)
    {
        return $"chat-{tenantID}-{sessionID}-{accessKey}";
    }

    public static string GetCacheKey(Guid tenantID, Guid sessionID, Guid accessKey, string topic, string key)
    {
        return $"chat-{tenantID}-{sessionID}-{accessKey}:{topic}={key}";
    }

    public Task<string> Get(string key)
    {
        return RGCache.Cache.Get(key);
    }

    public Task<bool> Put(string key, string value)
    {
        return RGCache.Cache.Put(key, value);
    }

    public Task<ChatCompletionResponse?> GetResponse (string key)
    {
        return RGCache.Cache.Get<ChatCompletionResponse>(key);
    }

    public Task<bool> PutResponse (string key, ChatCompletionResponse value)
    {
        return RGCache.Cache.Put<ChatCompletionResponse>(key, value);
    }

    public Task Remove(string key)
    {
        return RGCache.Cache.Remove(key);
    }

    public string GetMessageCacheKey(Guid tenantID, Guid sessionID, Guid accessKey)
    {
        return RGCache.GetCacheKey(tenantID, sessionID, accessKey);
    }

    public string GetEmbeddingCacheKey(string text)
    {
        return $"rg-embedding-{text.GetHashCode()}";
    }
}
