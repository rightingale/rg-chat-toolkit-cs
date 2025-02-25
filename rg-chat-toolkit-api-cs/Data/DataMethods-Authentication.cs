using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using rg_chat_toolkit_api_cs.Data.Models;

namespace rg_chat_toolkit_api_cs.Data;


public static partial class DataMethods
{
    public static List<Jwtauthorization>? JwtAuthentication_Select(Guid tenantID)
    {
        // Check cache
        string cacheKey = $"JwtAuthentication_Select_{tenantID}";
        if (cache.TryGetValue(cacheKey, out List<Jwtauthorization>? auths))
        {
            Console.WriteLine($"Memory_Get: Cache hit for {cacheKey}");
            return auths;
        }
        else
        {
            var val = JwtAuthentication_Select_Intern(tenantID);
            cache.Set(cacheKey, val, new MemoryCacheEntryOptions()
            {
                SlidingExpiration = SLIDING_EXPIRATION,
                AbsoluteExpiration = DateTime.Now + ABSOLUTE_EXPIRATION,
                Size = 1
            });
            return val;
        }
    }

    private static List<Jwtauthorization>? JwtAuthentication_Select_Intern(Guid tenantID)
    {
        var db = RGDatabaseContextFactory.Instance.CreateDbContext();

        var auths = db.Jwtauthorizations
            .Where(x => x.TenantId == tenantID)
            .ToList();

        return auths;
    }
}