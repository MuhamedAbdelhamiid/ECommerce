using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Domain.Contracts;
using StackExchange.Redis;

namespace ECommerce.Perstistence.Repositories
{
    public class CacheRepository : ICacheRepository
    {
        private readonly IDatabase _redisDatabase;

        public CacheRepository(IConnectionMultiplexer connection)
        {
            _redisDatabase = connection.GetDatabase();
        }

        public async Task<string?> GetAsync(string cacheKey) =>
            await _redisDatabase.StringGetAsync(cacheKey);

        public async Task SetAsync(string cacheKey, string value, TimeSpan ttl) =>
            await _redisDatabase.StringSetAsync(cacheKey, value, ttl);
    }
}
