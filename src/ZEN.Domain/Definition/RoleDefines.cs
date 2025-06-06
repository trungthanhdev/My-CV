namespace ZEN.Domain.Definition;

public class RoleMigrates
{
    public static readonly string[] Values = [
        "ADMIN",
        "USER",
    ];
}

public struct RoleDefines
{
    public const string ADMIN = "ADMIN";
    public const string USER = "USER";
}