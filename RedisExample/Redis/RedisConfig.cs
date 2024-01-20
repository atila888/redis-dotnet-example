namespace RedisExample.Redis;

public class RedisConfig
{
    public string Configuration { get; set; }
    public string Password { get; set; }
    public int LongTerm { get; set; }
    public int MidTerm { get; set; }
    public int ShortTerm { get; set; }
    public int DefaultTerm { get; set; }
    public string Prefix { get; set; }
    public bool RedisOn { get; set; }
}