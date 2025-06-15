
using Microsoft.Extensions.DependencyInjection;
using ZEN.Application.Core.Behaviors;
using FluentValidation;
using ZEN.Domain.Services;
using ZEN.Application.Services;
using ZEN.Domain.Common.Authenticate;
using ZEN.Application.Core.Authenticate;

namespace ZEN.Application;

public static class Application
{
    public static void AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(Application).Assembly;
        services.AddValidatorsFromAssembly(assembly);
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assembly);
            // config.AddOpenBehavior(typeof(ValidationBehaviour<,>));
            config.AddOpenBehavior(typeof(UnitOfWorkBehavior<,>));
        });

        services.AddHttpContextAccessor();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserIdentifierProvider, UserIdentitfierProvider>();
    }
}