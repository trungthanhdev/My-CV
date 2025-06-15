using System.Text.Json.Serialization;

namespace ZEN.Infrastructure.Integrations.ProvinceOpenAPI.DTO;

public class GetProvinceDTO
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
    public int Code { get; set; }
    
    // ....
}