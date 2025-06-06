
using Asp.Versioning.Builder;
using CTCore.DynamicQuery.Common.Definations;
using CTCore.DynamicQuery.Core.Primitives;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace ZEN.Controller.Extensions;

public interface IEndpoint
{
    IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints, ApiVersionSet version);
}

public static class EndpointExntensions
{
    public static readonly string BASE_ROUTE = "/api/v{version:apiVersion}";
    static readonly List<IEndpoint> registeredEndpoints = [];
    public static IServiceCollection RegisterEndpoints(this IServiceCollection services)
    {
        var modules = DiscoverEndpoints();
        foreach (var module in modules)
        {
            registeredEndpoints.Add(module);
        }
        return services;
    }
    public static WebApplication MapEndpoints(this WebApplication app, ApiVersionSet version)
    {
        foreach (var module in registeredEndpoints)
        {
            module.MapEndpoints(app, version);
        }
        return app;
    }

    private static IEnumerable<IEndpoint> DiscoverEndpoints()
    {
        return typeof(IEndpoint).Assembly
            .GetTypes()
            .Where(p => p.IsClass && p.IsAssignableTo(typeof(IEndpoint)))
            .Select(Activator.CreateInstance)
            .Cast<IEndpoint>();
    }

    public static IResult ToOk<TResult>(
       this CTBaseResult<TResult> result, Func<TResult, IResult> mapper
    )
    {
        return result.Match(
            su => mapper(su),
            exp => exp.ErrorCode switch
            {
                // ErrorCodes.BadRequest => Results.BadRequest(exp.ToErrorResponse()),
                // ErrorCodes.NotFound => Results.NotFound(exp.ToErrorResponse()),
                _ => Results.UnprocessableEntity(exp.ToErrorResponse())
            }
        );
    }
}