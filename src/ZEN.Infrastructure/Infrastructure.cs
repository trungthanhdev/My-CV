
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZEN.Infrastructure.Persistence;
using ZEN.Infrastructure.Core.Mapping;
using Microsoft.EntityFrameworkCore;
using ZEN.Infrastructure.Integrations;
using AutoMapper;
using CTCore.DynamicQuery.Core.Domain.Interfaces;
using ZEN.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using ZEN.Infrastructure.Extentions;
using ZEN.Infrastructure.Mysql;
using ZEN.Infrastructure.Mysql.Persistence;
using ZEN.Domain.Services;
using ZEN.Infrastructure.InSystemProvider;
using ZEN.Domain.Definition;

namespace ZEN.Infrastructure;

public static class Infrastructure
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var opts = new DbContextOptionsBuilder();

        if (!RuntimeConfig.IsOnPremise)
        {
            services.ApplyNeonInfrast(configuration);
            // services.ApplyMysqlInfrast(configuration);
            services.AddScoped<IUnitOfWork, UnitOfWork<AppDbContext>>();
            services.ApplyIdentityBuilder<AppDbContext>();
        }

        services.RegisterMapsterConfiguration();
        services.AddScoped<ProvinceOpenAPIService>();

        #region use for ctcore.q
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });
        services.AddSingleton(mapperConfig.CreateMapper());

        services.AddScoped<IHardwareSpec, HardwareSpecService>();
        services.AddScoped<IVAdminApiClient, VAdminApiClient>();
        #endregion
    }

    private static void ApplyIdentityBuilder<TContext>(this IServiceCollection services)
        where TContext : DbContext
    {
        services.AddTransient<IEmailSender<AspUser>, NullEmailSender>();
        services.AddIdentityCore<AspUser>(opt =>
        {
            opt.User.RequireUniqueEmail = true;
            opt.SignIn.RequireConfirmedEmail = false;
            opt.SignIn.RequireConfirmedAccount = false;
            opt.Password.RequireDigit = false;
            opt.Password.RequiredLength = 3;
            opt.Password.RequireUppercase = false;
            opt.Password.RequireNonAlphanumeric = false;
            opt.Password.RequireLowercase = false;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<TContext>()
        .AddSignInManager();
    }
}
