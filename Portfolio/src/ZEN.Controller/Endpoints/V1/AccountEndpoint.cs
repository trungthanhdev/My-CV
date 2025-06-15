
using System.Security.Claims;
using Asp.Versioning.Builder;
using CTCore.DynamicQuery.Core.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ZEN.Contract.AspAccountDto;
using ZEN.Controller.Extensions;
using ZEN.Domain.Common.Authenticate;
using ZEN.Domain.Definition;
using ZEN.Domain.Entities;
using ZEN.Domain.Services;
using ZEN.Domain.Services.Dtos;
namespace ZEN.Controller.Endpoints.V1;

public class AccountEndpointV1 : IEndpoint
{
    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints, ApiVersionSet version)
    {
        var defaultPath = $"{EndpointExntensions.BASE_ROUTE}";
        var acc = endpoints
            .MapGroup($"{defaultPath}/account")
            .WithDisplayName("Account")
            .WithApiVersionSet(version)
            .HasApiVersion(1);
        acc.MapPost("/dev-register", RegisterAsync);
        acc.MapPost("/log-in", LoginAsync);
        var auth = endpoints
            .MapGroup($"{defaultPath}/token")
            .WithDisplayName("Authenticate")
            .WithApiVersionSet(version)
            .HasApiVersion(1);

        auth.MapPost("/refresh", RefreshTokenAsync);
        auth.MapGet("/claims", (
            ClaimsPrincipal claimsPrincipal
        ) =>
        {
            return claimsPrincipal.Claims.ToDictionary(e => e.Type, c => c.Value);
        }).RequireAuthorization();

        auth.MapPost("/remove-tag/{tag}", async (
            [FromServices] HybridCache remoteCache,
            string tag
        ) =>
        {
            await remoteCache.RemoveByTagAsync(tag);
        });

        return endpoints;
    }

    private async Task<IResult> RefreshTokenAsync(
        HttpContext context,
        [FromServices] ITokenService tokenService,
        [FromBody] RFTokenRequest req
    )
    {
        var result = await tokenService.RefreshTokenAsync(req.RefreshToken, context.GetIpAddress());
        return Results.Ok(result);
    }
    private async Task<IResult> LoginAsync(
        [FromServices] UserManager<AspUser> userManager,
        [FromServices] SignInManager<AspUser> signInManager,
        [FromServices] ILogger<AccountEndpointV1> logger,
        [FromServices] ITokenService tokenService,
        HttpContext context,
        [FromServices] IOptions<JwtSettings> jwtSettings,
        [FromServices] HybridCache remoteCache,
        [FromBody] LoginDto loginDto
    )
    {
        if (string.IsNullOrWhiteSpace(loginDto.Username) || string.IsNullOrWhiteSpace(loginDto.Password))
        {
            return Results.BadRequest(new { message = "Username and password cannot be empty." });
        }

        var userID = await remoteCache.GetOrCreateAsync($"signin_{loginDto.Username}",
            async _ => await userManager.FindByNameAsync(loginDto.Username)
                ?? await userManager.FindByEmailAsync(loginDto.Username)
            , tags: [
                $"{CachingTags.SIGNIN(loginDto.Username)}"
            ]);
        var user = await userManager.FindByNameAsync(loginDto.Username);
        if (user == null)
        {
            logger.LogWarning("Login failed: User {Username} not found", loginDto.Username);
            // return Results.Unauthorized();
            return Results.Json(new { message = $"Login failed: User {loginDto.Username} not found" }, statusCode: 401);
        }

        // Check password
        var result = await signInManager.CheckPasswordSignInAsync(
            user, loginDto.Password, lockoutOnFailure: true);

        if (result.Succeeded)
        {
            logger.LogInformation("User {Username} logged in successfully", user.UserName);
            string secChUaPlatform = context.Request.Headers["sec-ch-ua-platform"].ToString() ?? "all";
            var genKey = $"token_{user.Id}_{context.GetIpv4Address()}_{secChUaPlatform}";
            var token = await remoteCache.GetOrCreateAsync(
                genKey,
                async _ =>
                {
                    var isKyb = false;

                    logger.LogInformation("Caching gen token with key {genKey} using user {Username}", genKey, user.UserName);
                    return await tokenService.GenerateTokensAsync(user, context.GetIpv4Address(), isKyb);
                }, new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromMinutes(jwtSettings.Value.AccessTokenExpirationMinutes - 1)
                }, [$"{CachingTags.TOKEN(user.Id)}"]);

            TimeSpan remainingTime = token.CreatedAt.AddMinutes(jwtSettings.Value.AccessTokenExpirationMinutes) - DateTime.UtcNow;
            int newExpiresIn = (int)Math.Max(0, remainingTime.TotalSeconds);
            token.ExpiresIn = newExpiresIn;
            var response = new { token, user_name = user.UserName, user_id = user.Id };
            return Results.Ok(response);
        }

        if (result.IsLockedOut)
        {
            logger.LogWarning("User {Username} is locked out", user.UserName);
            return Results.StatusCode(StatusCodes.Status423Locked);
        }

        if (result.RequiresTwoFactor)
        {
            return Results.StatusCode(StatusCodes.Status428PreconditionRequired);
        }

        logger.LogWarning("Login failed for user {Username}: Invalid password", loginDto.Username);
        return Results.Json(new { message = $"Login failed: Invalid password" }, statusCode: 401);
        // return Results.Unauthorized();
    }




    private async Task<IResult> RegisterAsync(
        [FromServices] UserManager<AspUser> userManager,
        RegisterDto model

    )
    {
        if (string.IsNullOrEmpty(model.Username) ||
            string.IsNullOrEmpty(model.Password))
        {
            return Results.BadRequest("Username, email, and password are required");
        }

        // Check if username already exists
        var existingUser = await userManager.FindByNameAsync(model.Username);
        if (existingUser != null)
        {
            return Results.BadRequest("Username already exists");
        }

        // Check if email already exists
        existingUser = model.Email != null ? await userManager.FindByEmailAsync(model.Email) : null;
        if (existingUser != null)
        {
            return Results.BadRequest("Email already exists");
        }

        // Create user
        var user = new AspUser
        {
            UserName = model.Username,
            Email = model.Email ?? model.Username + "@platform.vikhang"
        };

        var result = await userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            return Results.Ok(new
            {
                userId = user.Id
            });
        }

        return Results.BadRequest(result.Errors);
    }
}
