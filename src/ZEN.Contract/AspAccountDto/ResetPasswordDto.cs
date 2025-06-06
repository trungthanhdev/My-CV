using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ZEN.Contract.AspAccountDto
{
    public class ResetPasswordDto
    {
        [Required]
        public string? user_name { get; set; } = string.Empty;

    }
}