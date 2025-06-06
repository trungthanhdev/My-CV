
namespace ZEN.Domain.Services.Dtos;

public class TokenResponse
{
    public string RefreshToken { get; set; } = default!;
    public string AccessToken { get; set; } = default!;
    public int ExpiresIn { get; set; }
    public string TokenType { get; set; } = "Bearer";
    public DateTime CreatedAt { get; set; }
}

public class RFTokenRequest
{
    public string RefreshToken { get; set; } = default!;
}