
namespace ZEN.Contract.AspAccountDto;



public class SignUpDTO
{
    public string? FullName { get; set; }

    public string? Address { get; set; }

    public required string CompanyName { get; set; }

    public required string UserName { get; set; }

    public required string Password { get; set; }

    public string? Email { get; set; }

    public string? KeySeed { get; set; }
}
