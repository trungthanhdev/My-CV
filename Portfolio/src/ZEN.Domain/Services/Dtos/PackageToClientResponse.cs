
namespace ZEN.Domain.Services.Dtos;

public class PackageToClientResponse
{
    public string OrgId { get; set; } = default!;
    public string PackageName { get; set; } = default!;
    public string? Mac { get; set; }
    public int SoNguoi { get; set; } 
    public int SoMCC { get; set; }
    public long Exp { get; set; }
}
