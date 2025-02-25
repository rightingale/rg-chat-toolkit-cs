using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using rg_chat_toolkit_api_cs.Data.Models;

namespace rg_chat_toolkit_api_cs.Data;


public static partial class DataMethods
{
    public static Memory? Memory_Get(Guid tenantID, string name)
    {
        // Check cache
        string cacheKey = $"Memory_Get_{tenantID}_{name}";
        if (cache.TryGetValue(cacheKey, out Memory memory))
        {
            Console.WriteLine($"Memory_Get: Cache hit for {cacheKey}");
            return memory;
        }
        else
        {
            var val = Memory_Get_Intern(tenantID, name);
            cache.Set(cacheKey, val, new MemoryCacheEntryOptions()
            {
                SlidingExpiration = SLIDING_EXPIRATION,
                AbsoluteExpiration = DateTime.Now + ABSOLUTE_EXPIRATION,
                Size = 1
            });
            return val;
        }
    }

    private static Memory? Memory_Get_Intern(Guid tenantID, string name)
    {
        var db = RGDatabaseContextFactory.Instance.CreateDbContext();

        var memory = db.Memories
            .Where(m => m.TenantId == tenantID && m.Name == name)
            .SingleOrDefault();

        return memory;
    }
}