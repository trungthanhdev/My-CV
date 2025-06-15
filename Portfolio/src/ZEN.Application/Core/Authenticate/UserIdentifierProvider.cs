using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ZEN.Domain.Common.Authenticate;

namespace ZEN.Application.Core.Authenticate;

public class UserIdentitfierProvider(IHttpContextAccessor httpContextAccessor
    ) : IUserIdentifierProvider
{
    // public string AgencyId
    // {
    //     get
    //     {
    //         var token = httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
    //             .ToString().Replace("Bearer ", "");
    //         if (string.IsNullOrEmpty(token))
    //             return string.Empty;
    //         var handler = new JwtSecurityTokenHandler();
    //         var jwtToken = handler.ReadToken(token) as JwtSecurityToken;
    //         var agencyId = jwtToken?.Claims
    //                          ?.Where(c => c.Type == "nameid")
    //                          .Select(c => c.Value)
    //                          .FirstOrDefault() ?? string.Empty;
    //         return agencyId;
    //     }
    // }
    // public string UserId => httpContextAccessor.HttpContext?.User.Claims
    //             ?.Where(c => c.Type == "userId")
    //             .Select(c => c.Value)
    //             .First()!;
    // public string UnitId => httpContextAccessor.HttpContext?.User.Claims
    //             ?.Where(c => c.Type == "unitId")
    //             .Select(c => c.Value)
    //             .First()!;
    // public string Role => httpContextAccessor.HttpContext?.User.Claims
    //             ?.Where(c => c.Type == ClaimTypes.Role)
    //             .Select(c => c.Value)
    //             .First()!;
    // public string Name => httpContextAccessor.HttpContext?.User.Claims
    //             ?.Where(c => c.Type == "name")
    //             .Select(c => c.Value)
    //             .First()!;
    // public bool IsClerical => bool.Parse(httpContextAccessor.HttpContext?.User.Claims
    //             ?.Where(c => c.Type == "IsClerical")
    //             .Select(c => c.Value)
    //             .FirstOrDefault() ?? "false");
    public string OrgId => httpContextAccessor.HttpContext?.User.Claims
        ?.Where(c => c.Type == "org")
        .Select(c => c.Value)
        .FirstOrDefault() ?? string.Empty;

    public string UserId => httpContextAccessor.HttpContext?.User.Claims
        ?.Where(c => c.Type == ClaimTypes.NameIdentifier)
        .Select(c => c.Value)
        .FirstOrDefault() ?? string.Empty;
}
