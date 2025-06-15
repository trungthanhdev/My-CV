
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
using ZEN.Domain.Interfaces;
using ZEN.Infrastructure.Integrations.CloudStorage;
using ZEN.Infrastructure.Integrations.SendMail;
// using Microsoft.Extensions.Caching.StackExchangeRedis;
using ZEN.Infrastructure.Integrations.Redis;
using StackExchange.Redis;
using System.Security.Authentication;

namespace ZEN.Infrastructure;

public static class Infrastructure
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var opts = new DbContextOptionsBuilder();
        //---- Get redis connectionString -------

        // -----------------------

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
        services.AddScoped<ISavePhotoToCloud, SavePhotoToCloud>();
        services.AddScoped<ISendMail, SendMail>();

        #endregion

        // services.AddScoped<IRedisCache, RedisCache>();
        services.AddScoped<IRedisCache>(sp =>
        {
            var connStr = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING")!;
            return new RedisCache(connStr);
        });
        // services.AddStackExchangeRedisCache(options =>
        //    {
        //        options.Configuration = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING")!;
        //        options.InstanceName = "Portfolio_";
        //    });

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
