namespace RedisExample.Redis;

public interface ICache
{
    T Get<T>(string key);

    void Set<T>(string key, T obj, CacheDuration cacheDuration);
    void Delete(string key);

    bool Exists(string key);

    Task<bool> ExistsAsync(string key);
    Task<T> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T obj, CacheDuration cacheDuration);
    Task DeleteAsync(string key);
}