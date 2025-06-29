using StackExchange.Redis;
using ZEN.Domain.Interfaces;


namespace ZEN.Infrastructure.Integrations.Redis
{
    public class RedisCache : IRedisCache
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _db;

        public RedisCache(string redisConnectionString)
        {
            var options = ConfigurationOptions.Parse(redisConnectionString);

            options.User = Environment.GetEnvironmentVariable("REDIS_USER") ?? "default";

            options.AbortOnConnectFail = false;
            options.ConnectTimeout = 10000;
            options.SyncTimeout = 10000;
            options.KeepAlive = 60;
            options.Ssl = false;
            // Console.WriteLine($"User: {options.User}, Password: {options.Password}, EndPoints: {string.Join(",", options.EndPoints)}");

            _redis = ConnectionMultiplexer.Connect(options);
            _db = _redis.GetDatabase();
        }

        public async Task<string?> GetAsync(string key)
        {
            var value = await _db.StringGetAsync(key);
            return value.HasValue ? value.ToString() : null;
        }

        public async Task RemoveAsync(string key)
        {
            await _db.KeyDeleteAsync(key);
        }

        public async Task RemoveByPrefixAsync(string prefix)
        {
            var endpoints = _redis.GetEndPoints();
            foreach (var endpoint in endpoints)
            {
                var server = _redis.GetServer(endpoint);
                if (!server.IsConnected) continue;
                var keys = server.Keys(pattern: $"{prefix}*").ToArray();

                foreach (var key in keys)
                {
                    await _db.KeyDeleteAsync(key);
                }
            }
        }

        public async Task SetAsync(string key, string value, TimeSpan? expiry = null)
        {
            await _db.StringSetAsync(key, value, expiry);
        }
    }
}