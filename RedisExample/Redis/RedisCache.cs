using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace RedisExample.Redis;

public class RedisCache : ICache
{
    private readonly int _defaultTerm;
        private readonly IDistributedCache _distributedCache;

        private readonly int _longTerm;
        private readonly int _midTerm;
        private readonly RedisConfig _redisConfig;
        private readonly bool _redisOn;
        private readonly int _shortTerm;

        public RedisCache(IDistributedCache distributedCache, IOptions<RedisConfig> redisConfig)
        {
            _distributedCache = distributedCache;
            _redisConfig = redisConfig.Value;
            _defaultTerm = _redisConfig.DefaultTerm;
            _midTerm = _redisConfig.MidTerm;
            _shortTerm = _redisConfig.ShortTerm;
            _longTerm = _redisConfig.LongTerm;
            _redisOn = _redisConfig.RedisOn;
        }

        public void Set<T>(string key, T objectToCache, CacheDuration cacheDuration)
        {
            if (_redisOn)
            {
                var expireDate = GetExpireDate(cacheDuration);
                var expireTimeSpan = expireDate.Subtract(DateTime.Now);
                var prefixKey = string.Concat(_redisConfig.Prefix, key);
                _distributedCache.SetString(prefixKey, JsonConvert.SerializeObject(objectToCache),
                    new DistributedCacheEntryOptions {AbsoluteExpirationRelativeToNow = expireTimeSpan});
            }
        }

        public T Get<T>(string key)
        {
            T result = default;
            if (_redisOn)
            {
                var prefixKey = string.Concat(_redisConfig.Prefix, key);
                var redisObject = _distributedCache.GetString(prefixKey);
                result = redisObject != null ? JsonConvert.DeserializeObject<T>(redisObject) : default;
            }

            return result;
        }

        public void Delete(string key)
        {
            if (_redisOn)
            {
                var prefixKey = string.Concat(_redisConfig.Prefix, key);
                _distributedCache.Remove(prefixKey);
            }
        }

        public bool Exists(string key)
        {
            var result = false;
            if (_redisOn)
            {
                var prefixKey = string.Concat(_redisConfig.Prefix, key);
                result = !string.IsNullOrEmpty(_distributedCache.GetString(prefixKey));
            }

            return result;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var result = Task.FromResult(default(T));
            if (_redisOn)
            {
                var prefixKey = string.Concat(_redisConfig.Prefix, key);
                var redisObject = await _distributedCache.GetStringAsync(prefixKey);
                result = Task.FromResult(redisObject != null ? JsonConvert.DeserializeObject<T>(redisObject) : default);
            }

            return await result;
        }

        public async Task SetAsync<T>(string key, T objectToCache, CacheDuration cacheDuration)
        {
            if (_redisOn)
            {
                var expireDate = GetExpireDate(cacheDuration);
                var expireTimeSpan = expireDate.Subtract(DateTime.Now);
                var prefixKey = string.Concat(_redisConfig.Prefix, key);
                await _distributedCache.SetStringAsync(prefixKey, JsonConvert.SerializeObject(objectToCache),
                    new DistributedCacheEntryOptions {AbsoluteExpirationRelativeToNow = expireTimeSpan});
            }
        }

        public async Task DeleteAsync(string key)
        {
            if (_redisOn)
            {
                var prefixKey = string.Concat(_redisConfig.Prefix, key);
                await _distributedCache.RemoveAsync(prefixKey);
            }
        }

        public async Task<bool> ExistsAsync(string key)
        {
            var result = false;
            if (_redisOn)
            {
                var prefixKey = string.Concat(_redisConfig.Prefix, key);
                result = string.IsNullOrEmpty(await _distributedCache.GetStringAsync(prefixKey));
            }

            return result;
        }

        private DateTime GetExpireDate(CacheDuration cacheDuration)
        {
            DateTime expireDate;
            switch (cacheDuration)
            {
                case CacheDuration.Default:
                    expireDate = DateTime.Now.AddMinutes(_defaultTerm);
                    break;

                case CacheDuration.ShortTerm:
                    expireDate = DateTime.Now.AddMinutes(_shortTerm);
                    break;

                case CacheDuration.MidTerm:
                    expireDate = DateTime.Now.AddMinutes(_midTerm);
                    break;

                case CacheDuration.LongTerm:
                    expireDate = DateTime.Now.AddMinutes(_longTerm);
                    break;

                default:
                    expireDate = DateTime.Now.AddMinutes(_defaultTerm);
                    break;
            }

            return expireDate;
        }
}