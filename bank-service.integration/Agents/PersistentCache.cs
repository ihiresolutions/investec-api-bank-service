using bank_service.integration.contract.Agents;
using bank_service.integration.contract.Ioc.Options;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;

namespace bank_service.integration.Agents
{
    public class PersistentCache : ICache
    {
        #region Constructors

        public PersistentCache(IOptions<PersistentCacheOptions> options)
        {
            var redisOptions = new ConfigurationOptions
            {
                EndPoints = { $"{options.Value.Host}:{options.Value.Port}" },
                Password = options.Value.Password
            };

            var lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                return ConnectionMultiplexer.Connect(redisOptions);
            });

            _persistentCache = lazyConnection.Value.GetDatabase();
        }

        #endregion

        #region ICache Implementation

        public string GetCacheString(string cacheKey)
        {
            return _persistentCache.StringGet(cacheKey);
        }

        public void SetCacheString(string cacheKey, string value)
        {
            _persistentCache.StringSet(cacheKey, value);
        }

        #endregion

        #region Fields

        private readonly IDatabase _persistentCache;

        #endregion
    }
}
