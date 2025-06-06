namespace ZEN.Domain.Common.Authenticate;


public class JwtSettings
{
    public string Issuer { get; set; }= default!;
    public string Audience { get; set; }= default!;
    public int AccessTokenExpirationMinutes { get; set; } = 60; // 1 hour
    public int RefreshTokenExpirationDays { get; set; } = 7; // 7 days
}
