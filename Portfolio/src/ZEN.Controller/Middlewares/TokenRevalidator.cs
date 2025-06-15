using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using ZEN.Domain.Definition;
using ZEN.Domain.Services;

namespace ZEN.Controller.Middlewares;

public class TokenRevalidator(
    ILogger<TokenRevalidator> _logger,
    HybridCache _remoteCache,
    ITokenService _tokenService) : IMiddleware
{

    public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
    {
        var endpoint = httpContext.GetEndpoint();
        var isAuthorizeEndpoint = endpoint?.Metadata.GetMetadata<AuthorizeAttribute>() != null;

        if (isAuthorizeEndpoint)
        {
            try
            {
                // Extract token from Authorization header
                var rawToken = ExtractBearerToken(httpContext);

                // validate token 
                if (!string.IsNullOrEmpty(rawToken))
                {
                    var tokenValidator = _tokenService.ValidateToken(rawToken).Value;
                    var jit = tokenValidator.Claims.FirstOrDefault(e => e.Type == "jti")?.Value;
                    var userId = tokenValidator.Claims.FirstOrDefault(e => e.Type == ClaimTypes.NameIdentifier)?.Value;

                    var keyBlack = $"{CachingTags.BLACK_TOKEN(userId ?? "", jit ?? "")}";

                    // check black list
                    var isBlocked = await _remoteCache.ExistsAsync(keyBlack);

                    if (isBlocked)
                    {
                        throw new SecurityTokenNoExpirationException();
                    }
                }

                await next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred: {Message}", ex.Message);

                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;

                await using var writer = new Utf8JsonWriter(httpContext.Response.BodyWriter);
                writer.WriteStartObject();
                writer.WriteNumber("stCode", 401);
                writer.WriteString("msg", "Auth failed");
                writer.WriteEndObject();
                await writer.FlushAsync();
            }
        }
        else
        {
            await next(httpContext);
        }
    }

    private string ExtractBearerToken(HttpContext context)
    {
        // Check if Authorization header exists
        if (!context.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            return string.Empty;
        }

        var authHeaderValue = authHeader.ToString();

        // Check if it's a Bearer token
        if (!authHeaderValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return string.Empty;
        }

        // Extract the token part (remove "Bearer " prefix)
        return authHeaderValue.Substring(7).Trim();
    }
}
