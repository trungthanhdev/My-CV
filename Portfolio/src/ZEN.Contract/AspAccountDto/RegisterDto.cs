using System.ComponentModel.DataAnnotations;
// using ZEN.Domain.Common.Enum;

namespace ZEN.Contract.AspAccountDto;


public class RegisterDto
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Username { get; set; } = default!;
    [StringLength(255)]
    public string? FullName { get; set; }
    public string? Email { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = default!;
    // public string? Role { get; set; } = ERole.RECEIPTER.ToString();
    // public bool isAdminRegister { get; set; } = false;
}