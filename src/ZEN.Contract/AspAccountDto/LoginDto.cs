using System.ComponentModel.DataAnnotations;

namespace ZEN.Contract.AspAccountDto;

public class LoginDto
{
    [Required]
    public string Username { get; set; } = default!;

    [Required]
    public string Password { get; set; } = default!;

    // public bool? RememberMe { get; set; }
}