using rg_chat_toolkit_cs.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestHarness;

/// <summary>
/// Dummy implementation as Dictionary
/// </summary>
internal class RGCache : IRGEmbeddingCache
{
    protected static Dictionary<string, string> cache = new Dictionary<string, string>();

    public Task<string> Get(string key)
    {
        if (cache.ContainsKey(key))
        {
            return Task.FromResult(cache[key]);
        }
        else
        {
            return Task.FromResult<string>(null);
        }
    }
    public string GetEmbeddingCacheKey(string text)
    {
        //return $"rg-embedding-{text.GetHashCode()}";
        //return $"rg-embedding-{text}";

        //var hashCode = String.Intern(text).GetHashCode();
        return $"rg-embedding-{text}";
        //return $"rg-embedding-{String.Intern(text).GetHashCode()}";
    }

    public Task<bool> Put(string key, string value)
    {
        cache[key] = value;
        return Task.FromResult(true);
    }

    public Task Remove(string key)
    {
        cache.Remove(key);
        return Task.CompletedTask;
    }
}
