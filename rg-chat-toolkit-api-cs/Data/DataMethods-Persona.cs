using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using rg_chat_toolkit_api_cs.Data.Models;

namespace rg_chat_toolkit_api_cs.Data;


public static partial class DataMethods
{
    public static Persona? Persona_Get(Guid tenantID, string promptName, string name)
    {
        // Check cache
        string cacheKey = $"Persona_Get_{tenantID}_{name}";
        if (cache.TryGetValue(cacheKey, out Persona persona))
        {
            Console.WriteLine($"Persona_Get: Cache hit for {cacheKey}");
            return persona;
        }
        else
        {
            var val = Persona_Get_Intern(tenantID, promptName, name);
            cache.Set(cacheKey, val, new MemoryCacheEntryOptions()
            {
                SlidingExpiration = SLIDING_EXPIRATION,
                AbsoluteExpiration = DateTime.Now + ABSOLUTE_EXPIRATION,
                Size = 1
            });
            return val;
        }
    }

    private static Persona? Persona_Get_Intern(Guid tenantID, string promptName, string name)
    {
        var db = RGDatabaseContextFactory.Instance.CreateDbContext();

        var prompt = db.Prompts
            .Include(p => p.PromptPersonas)
            .ThenInclude(pp => pp.Persona)
            .FirstOrDefault(p => p.TenantId == tenantID && p.Name == promptName);

        var persona = prompt?.PromptPersonas
            .FirstOrDefault(pp => pp.Persona.Name == name)?.Persona;

        return persona;
    }
}