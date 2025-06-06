using System.Net.Http.Json;
using ZEN.Infrastructure.Integrations.ProvinceOpenAPI.DTO;

namespace ZEN.Infrastructure.Integrations;

public class ProvinceOpenAPIService(HttpClient httpClient)
{
    public async Task<IEnumerable<GetProvinceDTO>?> GetProvincesAsync(CancellationToken cancellationToken)
    => await httpClient
            .GetFromJsonAsync<IEnumerable<GetProvinceDTO>>($"/api/p/?depth=1", cancellationToken);
    
    public async Task<IEnumerable<GetDistrictDTO>?> GetDistrictAsync(CancellationToken cancellationToken)
    => await httpClient
            .GetFromJsonAsync<IEnumerable<GetDistrictDTO>>($"/api/d/?depth=1", cancellationToken);
    
    public async Task<IEnumerable<GetWardDTO>?> GetWardAsync(CancellationToken cancellationToken)
    => await httpClient
            .GetFromJsonAsync<IEnumerable<GetWardDTO>>($"/api/w/?depth=1", cancellationToken);
}