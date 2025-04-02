using System.ComponentModel.DataAnnotations;
namespace Domain.DTO
{
    public class UserForRegistrationDto
    {
        [Required(ErrorMessage = "Введите имя")]
        public string? FirstName { get; set; }
        [Required(ErrorMessage = "Введите фамилию")]
        public string? LastName { get; set; }
        [Required(ErrorMessage = "Введите отество")]
        public string? FatherName { get; set; }
        [Required(ErrorMessage ="Введите email")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Введите пароль")]
        public string? Password { get; set; }
        [Compare("Password",ErrorMessage = "Пароли не совпадают")]
        public string? ConfirmPassword { get; set; }

        public string? ClientUri {  get; set; }
    }
}
