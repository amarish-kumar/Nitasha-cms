using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NITASA.Core.Caching
{
    public interface ICacheManager
    {
        T Get<T>(string key);

        T Set<T>(string key, object data, int cacheTime);

        bool IsSet(string key);

        void Remove(string key);

        void Clear();
    }
}