
using System.Security.Claims;
using Microsoft.Extensions.Options;
using ZEN.Domain.Entities;
using ZEN.Domain.Services.Dtos;

namespace ZEN.Domain.Services;

public interface ITokenService
{
    Task<TokenResponse> GenerateTokensAsync(AspUser user, string ipAddress, bool isKyb);
    Task<TokenResponse> RefreshTokenAsync(string refreshToken, string ipAddress);
    Task RevokeTokenAsync(string refreshToken, string ipAddress);
    IOptions<ClaimsPrincipal> ValidateToken(string token);
}
