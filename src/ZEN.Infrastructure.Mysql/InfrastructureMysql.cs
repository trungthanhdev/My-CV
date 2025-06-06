
using CTCore.DynamicQuery.Core.Domain.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZEN.Domain.Definition;
using ZEN.Domain.Entities;
using ZEN.Infrastructure.Mysql.Persistence;
using ZEN.Infrastructure.Mysql.Persistence.Repositories;

namespace ZEN.Infrastructure.Mysql;

public static class InfrastructurePostgres
{
    public static void ApplyNeonInfrast(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

        services.AddDbContext<IdentityDbContext<AspUser>, AppDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
            options.EnableSensitiveDataLogging(RuntimeConfig.IsDbLogging);
        });

        services.AddScoped(typeof(IRepository<>), typeof(MainRepository<>));
    }
}

