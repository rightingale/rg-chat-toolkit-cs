using epic_retail_api_cs.Cache;
using rg_chat_toolkit_api_cs.Chat;

namespace rg_chat_toolkit_api_cs.Cache;

public static class RGCache
{
    private static readonly IConfigurationRoot config = new ConfigurationManager()
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
        .AddUserSecrets<ChatCompletionController>()
        .AddEnvironmentVariables()
        .Build();

    public static readonly RedisCacheService Cache;

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
}
