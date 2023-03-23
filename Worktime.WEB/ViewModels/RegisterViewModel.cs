using System.ComponentModel.DataAnnotations;

namespace Worktime.WEB.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = EMPTY_ERROR)]
        [Display(Name = "Email")]
        [RegularExpression(@"^[A-Za-z-_0-9\.]{2,25}@[A-Za-z0-9-]+\.[A-Za-z0-9-]+$", ErrorMessage = EMAIL_ERROR)]
        [MaxLength(50, ErrorMessage = EMAIL_ERROR)]
        public string? Email { get; set; }

        [Required(ErrorMessage = EMPTY_ERROR)]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Длина не должна быть меньше 6 символов!")]
        [Display(Name = "Пароль")]
        public string? Password { get; set; }

        [Required(ErrorMessage = EMPTY_ERROR)]
        [Compare("Password", ErrorMessage = "Пароль и подтверждение пароля должны быть одинаковы!")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердите пароль")]
        public string? PasswordConfirm { get; set; }

        private const string EMPTY_ERROR = "Заполните поле!";
        private const string EMAIL_ERROR = "Некорректный адрес электронной почты!";
    }
}
