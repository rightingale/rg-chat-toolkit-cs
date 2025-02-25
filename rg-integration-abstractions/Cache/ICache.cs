using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rg_chat_toolkit_cs.Cache;

public interface IRGCache
{
    Task<string> Get(string key);

    Task<bool> Put(string key, string value);

    Task Remove(string key);
}

public interface IRGMessageCache: IRGCache
{
    string GetMessageCacheKey(Guid tenantID, Guid sessionID, Guid accessKey);
}

public interface IRGEmbeddingCache : IRGCache
{
    string GetEmbeddingCacheKey(string text);
}