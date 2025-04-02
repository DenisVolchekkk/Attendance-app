using System.ComponentModel.DataAnnotations;
namespace Domain.DTO
{
    public class UserForAuthenticationDto
    {
        [Required(ErrorMessage = "Введите email")]
        public string? Email {  get; set; }
        [Required(ErrorMessage = "Введите пароль")]
        public string? Password { get; set; }
    }
}
