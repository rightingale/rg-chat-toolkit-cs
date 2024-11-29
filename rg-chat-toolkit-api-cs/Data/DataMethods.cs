using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using rg_chat_toolkit_api_cs.Data.Models;

namespace rg_chat_toolkit_api_cs.Data;

public static partial class DataMethods
{

    //public static readonly Guid PRODUCER_TOKEN_SYSTEM = Guid.Parse("00000000-0000-0000-0000-000000000000");

    public static readonly TimeSpan SLIDING_EXPIRATION = TimeSpan.FromMinutes(15);
    public static readonly TimeSpan ABSOLUTE_EXPIRATION = TimeSpan.FromHours(8);

    private static MemoryCache cache = new MemoryCache(new MemoryCacheOptions()
    {
        SizeLimit = 2000
    });

    public static Prompt? Prompt_Get(Guid tenantID, string name)
    {
        // Check cache
        string cacheKey = $"Prompt_Get_{tenantID}_{name}";
        if (cache.TryGetValue(cacheKey, out Prompt prompt))
        {
            Console.WriteLine($"Prompt_Get: Cache hit for {cacheKey}");
            return prompt;
        }
        else
        {
            var val = Prompt_Get_Intern(tenantID, name);
            cache.Set(cacheKey, val, new MemoryCacheEntryOptions() {
                SlidingExpiration = SLIDING_EXPIRATION,
                AbsoluteExpiration = DateTime.Now + ABSOLUTE_EXPIRATION,
                Size = 1
            });
            return val;
        }
    }

    private static Prompt? Prompt_Get_Intern(Guid tenantID, string name)
    {
        var db = RGDatabaseContextFactory.Instance.CreateDbContext();
        var returnVal = db.Prompts
            .Include(p => p.ReponseContentTypeNameNavigation)
            .Include(p => p.PromptMemories)
            .ThenInclude(pm => pm.Memory)
            .Where(p =>
                p.TenantId == tenantID
                && p.Name == name
            )
            .FirstOrDefault();

        return returnVal;
    }

}
