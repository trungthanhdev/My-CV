namespace ZEN.Domain.Definition;

public struct CachingTags
{
    public static string BLACK_TOKEN(string key, string jit) => $"BLACK_TOKEN_{key}_{jit}";
    public static string TOKEN(string usi) => $"TOKEN_{usi}";
    public static string SIGNIN(string userName) => $"SIGNIN_{userName}";

    public static string COMPANY(string orgId) => $"COMPANY_{orgId}"; // get page company
    public static string AREA(string orgId) => $"AREA_{orgId}"; // get page company
    public static string DEPART(string orgId) => $"DEPART_{orgId}"; // get page company
    public static string POSITION(string orgId) => $"POSITION_{orgId}"; // get page company
}