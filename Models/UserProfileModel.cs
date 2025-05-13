using System.ComponentModel.DataAnnotations;

namespace final_qualifying_work.Models
{
    public class UserProfileModel
    {
        [Required]
        [Display(Name = "Никнейм")]
        [StringLength(50, ErrorMessage = "Никнейм должен быть от {2} до {1} символов.", MinimumLength = 3)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Текущий пароль")]
        public string CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль")]
        [StringLength(100, ErrorMessage = "Пароль должен быть от {2} до {1} символов.", MinimumLength = 6)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтвердите новый пароль")]
        [Compare("NewPassword", ErrorMessage = "Пароли не совпадают.")]
        public string ConfirmPassword { get; set; }
    }
}
