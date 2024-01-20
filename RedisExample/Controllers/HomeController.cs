using Microsoft.AspNetCore.Mvc;
using RedisExample.Redis;

namespace RedisExample.Controllers;

[ApiController]
[Route("[controller]")]
public class HomeController : ControllerBase
{
    private readonly ICache Cache;
    public HomeController(ICache cache)
    {
        Cache = cache;
    }
    [HttpPost("api/index")]
    public async Task<string> Index()
    {
        var key = "Redis";
        var result = string.Empty;
        if (Cache.Exists(key))
        {
            result = "Coming from Redis." + Cache.Get<string>(key);
        }
        else
        {
            result = DateTime.Now.ToString();
            Cache.Set(key, result, CacheDuration.Default);
        }

        return await Task.FromResult(result);
    }
}