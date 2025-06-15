using System.Text.Json.Serialization;

namespace ZEN.Infrastructure.Integrations.ProvinceOpenAPI.DTO;

public class GetWardDTO
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
    public int Code { get; set; }
    [JsonPropertyName("district_code")]
    public int DistrictCode { get; set; }
    // ....
}