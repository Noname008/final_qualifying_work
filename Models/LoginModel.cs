using System.ComponentModel.DataAnnotations;

namespace final_qualifying_work.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Требуется email или никнейм")]
        [Display(Name = "Email или никнейм")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Пароль обязателен")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
