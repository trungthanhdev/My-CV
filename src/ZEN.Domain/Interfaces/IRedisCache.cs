using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZEN.Domain.Interfaces
{
    public interface IRedisCache
    {
        Task SetAsync(string key, string value, TimeSpan? expiry = null);
        Task<string?> GetAsync(string key);
        Task RemoveAsync(string key);
    }
}