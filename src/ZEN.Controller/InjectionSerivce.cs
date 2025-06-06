using ZEN.Infrastructure;
using ZEN.Application;
using System.Text.Json.Serialization;
using CTCore.DynamicQuery.OData;
using ZEN.Controller.Extensions;
using System.Text.Json;
using ZEN.Controller.Configurations;
using ZEN.Domain.Common.Authenticate;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Asp.Versioning.Builder;
using Asp.Versioning;
using ZEN.Controller.Middlewares;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using ZEN.Infrastructure.Extentions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using ZEN.Domain.Definition;
using ZEN.Controller.Endpoints.V1;

namespace ZEN.Controller;

public static class InjectionService
{
    public static void ApplyDInjectionService(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddLogging(
            builder =>
            builder.AddConsole());

        services
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

        services.AddApplication();
        services.AddInfrastructure(configuration);

        services.AddEndpointsApiExplorer();
        services.AddSwaggerConfigure();
        services.AddVersionApi();
        services.AddCorsConfig();
        services.AddJWTConfig(configuration);
        services.RegisterEndpoints();
        services.AddHttpClientConfig(configuration);

        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(
            options =>
            {
                options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            }
        );

        services
            .AddGenericODataEndpoints();

        if (!RuntimeConfig.IsOnPremise)
        {
            services.AddHealthChecks()
                .AddNpgSql(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")!, name: "maindb");
        }


        services.AddHybridCache(opt =>
        {
            opt.DefaultEntryOptions = new Microsoft.Extensions.Caching.Hybrid.HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromHours(1),

            };
        });
        if (RuntimeConfig.IsRedis)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING")!;
                options.InstanceName = "VTKCache_";
            });
        }
        services.AddScoped<TokenRevalidator>();
    }

    public static async Task ApplyWebBuilder(this WebApplication app)
    {

        app.UseGenericODataEndpoints();

        ApiVersionSet versionSet = app.NewApiVersionSet()
            .HasApiVersion(ApiVersion.Default)
            .ReportApiVersions()
            .Build();

        app.MapEndpoints(versionSet);
        new ProjectEndpoint().MapEndpoints(app, versionSet);


        if (true) //app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            await app.ApplyMigrations();
        }

        var uploadPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Uploads");
        if (!Directory.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath);
        }
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(uploadPath),
            RequestPath = "/uploads"
        });

        // app.UseOutputCache();
        app.UseHttpsRedirection();

        app.UseCors("CorsPolicy");

        app.UseAuthentication();

        app.UseMiddleware<TokenRevalidator>();
        app.UseMiddleware<CustomExceptionHandler>();

        app.UseAuthorization();

        app.MapControllers();

        // app.MapIdentityApi<AspUser>();
        if (!RuntimeConfig.IsOnPremise)
        {
            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";
                    var result = JsonSerializer.Serialize(new
                    {
                        status = report.Status.ToString(),
                        checks = report.Entries.Select(entry => new
                        {
                            name = entry.Key,
                            status = entry.Value.Status.ToString(),
                            exception = entry.Value.Exception?.Message,
                            duration = entry.Value.Duration.ToString()
                        })
                    });
                    await context.Response.WriteAsync(result);
                }
            });
        }
        else
        {
            app.MapGet("/health", () => "Ok.");
        }

    }
}