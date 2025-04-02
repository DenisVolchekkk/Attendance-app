using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Domain.DTO
{
    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "Пароль не введен")]
        public string? Password { get; set; }
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string? ConfirmPassword { get; set; }
        public string? Email { get; set; }
        public string? Token { get; set; }

    }
}
