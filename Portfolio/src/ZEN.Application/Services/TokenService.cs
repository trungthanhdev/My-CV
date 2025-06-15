using CTCore.DynamicQuery.Core.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ZEN.Domain.Common.Authenticate;
using ZEN.Domain.Definition;
using ZEN.Domain.Entities;
using ZEN.Domain.Services;
using ZEN.Domain.Services.Dtos;

namespace ZEN.Application.Services;

public class TokenService(
    ILogger<TokenService> _logger,
    UserManager<AspUser> _userManager,
    HybridCache remoteCache,
    IUnitOfWork _unitOfWork,
    IRepository<RefreshToken> _refreshTokenRepos,
    IOptions<JwtSettings> jwtSettings) : ITokenService
{
    private JwtSettings _jwtSettings => jwtSettings.Value;
    public async Task<TokenResponse> GenerateTokensAsync(AspUser user, string ipAddress, bool isKyb)
    {

        // Generate access token
        var (token, createdAt) = await GenerateAccessTokenAsync(user, isKyb);

        // Generate refresh token
        var refreshToken = GenerateRefreshToken(ipAddress);

        refreshToken.AspUserId = user.Id;

        // Save refresh token to user
        await SaveRefreshTokenAsync(user, refreshToken);

        return new TokenResponse
        {
            AccessToken = token,
            RefreshToken = refreshToken.Token,
            ExpiresIn = _jwtSettings.AccessTokenExpirationMinutes * 60,
            CreatedAt = createdAt
        };
    }

    public async Task<TokenResponse> RefreshTokenAsync(string refreshToken, string ipAddress)
    {
        var rf = await GetUserByRefreshTokenAsync(refreshToken) ?? throw new SecurityTokenException("Invalid refresh token or refresh token is used");

        // Validate refresh token
        if (!rf.IsActive)
            throw new SecurityTokenException("Refresh token expired or revoked");

        if (rf.CreatedByIp != ipAddress)
            throw new SecurityTokenException("Hacker right? not today :)");

        // Generate new tokens
        var newRefreshToken = GenerateRefreshToken(ipAddress);

        newRefreshToken.AspUserId = rf.AspUserId;

        // Revoke old refresh token
        rf.Revoked = DateTime.UtcNow;
        rf.RevokedByIp = ipAddress;
        rf.ReplacedByToken = newRefreshToken.Token;

        // Add new refresh token
        _refreshTokenRepos.Add(newRefreshToken);

        await _unitOfWork.SaveChangeAsync();

        var isKyb = false;
        // Generate new access token
        var (token, createdAt) = await GenerateAccessTokenAsync(rf.AspUser!, isKyb);

        await remoteCache.RemoveByTagAsync(CachingTags.TOKEN(rf.AspUserId)).ConfigureAwait(false);
        return new TokenResponse
        {
            AccessToken = token,
            RefreshToken = newRefreshToken.Token,
            ExpiresIn = _jwtSettings.AccessTokenExpirationMinutes * 60,
            CreatedAt = createdAt
        };
    }

    public Task RevokeTokenAsync(string refreshToken, string ipAddress)
    {
        throw new NotImplementedException();
        // var user = await GetUserByRefreshTokenAsync(refreshToken)
        //     ?? throw new SecurityTokenException("Invalid refresh token");

        // var token = user.RefreshTokens.Single(x => x.Token == refreshToken);

        // if (!token.IsActive)
        //     throw new SecurityTokenException("Invalid refresh token");

        // // Revoke token
        // token.Revoked = DateTime.UtcNow;
        // token.RevokedByIp = ipAddress;

        // // Update user
        // await _userManager.UpdateAsync(user);
    }

    public IOptions<ClaimsPrincipal> ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var secret = Environment.GetEnvironmentVariable("JWT_SECRET");
        if (string.IsNullOrEmpty(secret))
        {
            _logger.LogError("Not found JWT_SECRET on env");
            throw new InvalidOperationException("Not found JWT_SECRET on env");
        }
        var key = Encoding.ASCII.GetBytes(secret);

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = _jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
            return Options.Create(principal);
        }
        catch (Exception ex)
        {
            _logger.LogError("Validate token error {Message}", ex.Message);
            throw;
        }
    }

    #region Private Helper Methods

    private async Task<(string token, DateTime createdAt)> GenerateAccessTokenAsync(AspUser user, bool isKyb = false)
    {

        var tokenHandler = new JwtSecurityTokenHandler();
        var secret = Environment.GetEnvironmentVariable("JWT_SECRET");
        if (string.IsNullOrEmpty(secret))
        {
            _logger.LogError("Not found JWT_SECRET on env");
            throw new InvalidOperationException("Not found JWT_SECRET on env");
        }
        var key = Encoding.ASCII.GetBytes(secret);

        // Get user roles
        var roles = await _userManager.GetRolesAsync(user);

        // Get user claims
        var userClaims = await _userManager.GetClaimsAsync(user);

        // Create claims for token
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            // new Claim("user_id", user.Id),
            // new Claim("user_name", user.UserName!),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new Claim("kyb", isKyb.ToString()),
            // new Claim("org", user.OrgId ?? "")
        };

        // Add user properties as claims if available
        // if (!string.IsNullOrEmpty(user.FirstName))
        //     claims.Add(new Claim(ClaimTypes.GivenName, user.FirstName));
        // if (!string.IsNullOrEmpty(user.LastName))
        //     claims.Add(new Claim(ClaimTypes.Surname, user.LastName));

        // Add roles as claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Add custom claims
        claims.AddRange(userClaims);

        var dt = DateTime.UtcNow;
        // Create token descriptor
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = dt.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return (tokenHandler.WriteToken(token), dt);
    }

    private RefreshToken GenerateRefreshToken(string ipAddress)
    {
        using var rng = RandomNumberGenerator.Create();
        var randomBytes = new byte[64];
        rng.GetBytes(randomBytes);

        return new RefreshToken
        {
            Token = Convert.ToBase64String(randomBytes),
            ExpireAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = ipAddress
        };
    }

    private async Task SaveRefreshTokenAsync(AspUser user, RefreshToken refreshToken)
    {
        _refreshTokenRepos.Add(refreshToken);
        await _unitOfWork.SaveChangeAsync();
    }

    private async Task<RefreshToken?> GetUserByRefreshTokenAsync(string refreshToken)
    {
        var getToken = await _refreshTokenRepos.BuildQuery
            .Include(e => e.AspUser)
            .Where(e => e.Revoked == null)
            .FirstOrDefaultAsync(e => e.Token == refreshToken);

        return getToken;
    }


    #endregion
}
