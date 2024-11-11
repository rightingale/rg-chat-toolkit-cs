using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace rg_chat_toolkit_api_cs.Cache
{
    public class CacheService
    {
        public static readonly TimeSpan SLIDING_EXPIRATION = TimeSpan.FromMinutes(15);
        public static readonly TimeSpan ABSOLUTE_EXPIRATION = TimeSpan.FromHours(1);
        public static readonly TimeSpan VOLATILE_EXPIRATION = TimeSpan.FromSeconds(8);
        public static readonly TimeSpan LOG_EXPIRATION = TimeSpan.FromDays(2);

        static readonly int SIZE_LIMIT = 2000;

        internal class CacheKey
        {
            public string SmociSection { get; set; }
            public string SmociModule { get; set; }
            public string SmociObject { get; set; }
            public string SmociQuery { get; set; }
            public Dictionary<string, string> Values { get; set; }
        }

        public static string GetChecksum(string tenant, object obj)
        {
            if (tenant?.Length == 0)
            {
                throw new ArgumentException(nameof(tenant));
            }

            // Serialize clientSessionRequest with NewtonSoft JSON
            string json = JsonConvert.SerializeObject(obj);
            return tenant + "-" + json;

            //using (SHA256 sha256Hash = SHA256.Create())
            //{
            //    byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(json));
            //    StringBuilder builder = new StringBuilder();
            //    for (int i = 0; i < bytes.Length; i++)
            //    {
            //        builder.Append(bytes[i].ToString("x2"));
            //    }
            //    return tenant + "-" + builder.ToString();
            //}
        }

        private static MemoryCache cache = new MemoryCache(new MemoryCacheOptions()
        {
            SizeLimit = SIZE_LIMIT
        });

        public async Task<T> GetOrCreate<T>(
            string smociSection,
            string smociModule,
            string smociObject,
            string smociQuery,
            Dictionary<string, string> values,
            Func<Task<T>> factory)
        {
            CacheKey key = new  ()
            {
                SmociSection = smociSection,
                SmociModule = smociModule,
                SmociObject = smociObject,
                SmociQuery = smociQuery,
                Values = values
            };

            var strKey = JsonConvert.SerializeObject(key, Formatting.None);

            return await this.GetOrCreate<T>(strKey, factory);
        }

        public virtual async Task<T> GetOrCreate<T>(object key, Func<Task<T>> factory)
        {
#if VERBOSE
            if (cache.Get(key) != null)
            {
                await LoggingService.LogInformation("MemCache HIT!! [" + key + "]");
            }
#endif

            Func<ICacheEntry, Task<T>> itemFactory = async (entry) =>
            {
#if VERBOSE
                Console.WriteLine("### Cache miss [" + key + "]");
#endif

                entry.Size = 1;
                entry.SlidingExpiration = SLIDING_EXPIRATION;
                entry.AbsoluteExpiration = (DateTime.Now + ABSOLUTE_EXPIRATION);

                return await factory.Invoke();
            };

            return await cache.GetOrCreateAsync<T>(key, itemFactory);
        }
    }

}