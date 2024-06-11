using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using rg_chat_toolkit_cs.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rg_chat_toolkit_cs.Cache
{
    internal class CacheManager
    {
        static readonly TimeSpan SLIDING_EXPIRATION = TimeSpan.FromMinutes(15);
        static readonly TimeSpan ABSOLUTE_EXPIRATION = TimeSpan.FromHours(1);

        static readonly int SIZE_LIMIT = 2000;

        internal class CacheKey
        {
            public Guid SessionID { get; set; }
        }

        private static MemoryCache cache = new MemoryCache(new MemoryCacheOptions()
        {
            SizeLimit = SIZE_LIMIT
        });

        public static T Populate<T>(
            Guid SessionID,
            T values)
        {
            CacheKey key = new CacheKey()
            {
                SessionID = SessionID
            };

            return cache.Set<T>(key, values, new MemoryCacheEntryOptions()
            {
                SlidingExpiration = SLIDING_EXPIRATION,
                AbsoluteExpirationRelativeToNow = ABSOLUTE_EXPIRATION
            });
        }

        public static T GetOrCreate<T>(
            Guid SessionID,
            List<Message> values,
            Func<T> factory)
        {
            CacheKey key = new CacheKey()
            {
                SessionID = SessionID
            };

            //Console.WriteLine("CacheKey:\n");
            //Console.WriteLine(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(key, Formatting.Indented))?.ToString());

            var strKey = JsonConvert.SerializeObject(key, Formatting.None);

            return CacheManager.GetOrCreate<T>(strKey, factory);
        }

        public static T GetOrCreate<T>(object key, Func<T> factory)
        {
#if DEBUG
            if (cache.Get(key) != null)
            {
                Console.WriteLine("### Cache HIT!!");
            }
#endif

            Func<ICacheEntry, T> itemFactory = (entry) =>
            {
#if DEBUG
                Console.WriteLine("### Cache miss [" + key + "]");
#endif

                entry.Size = 1;
                entry.SlidingExpiration = SLIDING_EXPIRATION;
                entry.AbsoluteExpiration = (DateTime.Now + ABSOLUTE_EXPIRATION);

                return factory.Invoke();
            };

            return cache.GetOrCreate<T>(key, itemFactory);
        }
    }
}
