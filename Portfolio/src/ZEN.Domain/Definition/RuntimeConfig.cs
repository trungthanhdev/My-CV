namespace ZEN.Domain.Definition;

public readonly struct RuntimeConfig
{
    public const int HybridCacheTimeDefault = 60;
    public static bool IsRedis => bool.Parse(Environment.GetEnvironmentVariable("REDIS_MODE") ?? "false");
    public static bool IsOnPremise => bool.Parse(Environment.GetEnvironmentVariable("ONPREMISE") ?? "false");
    public static bool IsDbLogging => bool.Parse(Environment.GetEnvironmentVariable("DB_LOGGING") ?? "False");
}