using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace NITASA.Services.Caching
{
    public class UserRoleCacheManager : ICacheManager
    {
        protected ObjectCache Cache
        {
            get
            {
                return MemoryCache.Default;
            }
        }

        public T Get<T>(string userName)
        {
            return (T)Cache[userName];
        }

        public void Set(string userName, object rights, int cacheTime)
        {
            if (rights == null)
                return;

            var policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(cacheTime);

            if (!IsSet(userName))
            {
                Cache.Add(userName, rights, policy);
            }
            else
            {
                Cache.Remove(userName);
                Cache.Set(userName, rights, policy);
            }
        }

        public bool IsSet(string key)
        {
            return Cache.Contains(key);
        }

        public void Remove(string key)
        {
            Cache.Remove(key);
        }

        public void Clear()
        {
            foreach (var item in Cache)
            {
                Remove(item.Key);
            }
        }
    }
}