using System.Text.Json.Serialization;

namespace ZEN.Infrastructure.Integrations.ProvinceOpenAPI.DTO;

public class GetDistrictDTO
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
    
    public int Code { get; set; }
    
    [JsonPropertyName("province_code")]
    public int ProvinceCode { get; set; }
    // ....
}