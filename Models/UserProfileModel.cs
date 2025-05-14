using System.ComponentModel.DataAnnotations;

namespace final_qualifying_work.Models
{
    public class UserProfileModel
    {
        [Required(ErrorMessage = "Никнейм обязателен")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Никнейм должен быть от 3 до 20 символов")]
        [Display(Name = "Никнейм")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Текущий пароль")]
        public string? CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль должен быть от 6 до 100 символов")]
        [Display(Name = "Новый пароль")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтвердите пароль")]
        [Compare("NewPassword", ErrorMessage = "Пароли не совпадают")]
        public string? ConfirmPassword { get; set; }
    }
}
