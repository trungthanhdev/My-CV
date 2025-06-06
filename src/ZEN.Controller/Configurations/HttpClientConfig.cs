using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZEN.Infrastructure.Integrations;

namespace ZEN.Controller.Configurations;

public static class HttpClientConfig
{
    public static void AddHttpClientConfig(this IServiceCollection services, IConfiguration configuration)
    {
        var provinceUrl = configuration["ProvinceOpenAPI:HostName"];
        services.AddHttpClient<ProvinceOpenAPIService>("province_openapi", (client) =>
        {
            client.BaseAddress = new Uri(provinceUrl ?? "https://provinces.open-api.vn");
        });

    }
}
