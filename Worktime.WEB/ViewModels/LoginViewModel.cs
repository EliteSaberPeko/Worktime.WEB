using System.ComponentModel.DataAnnotations;

namespace Worktime.WEB.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Заполните поле!")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "EMail")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Заполните поле!")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string? Password { get; set; }

        [Display(Name = "Запомнить?")]
        public bool RememberMe { get; set; }
    }
}
