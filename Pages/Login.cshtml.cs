using final_qualifying_work.Data;
using final_qualifying_work.Models;
using final_qualifying_work.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace final_qualifying_work.Pages
{
    public class LoginPageModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;

        public string Token { get; private set; }

        public LoginPageModel(AppDbContext context, JwtService jwtService)
        {
            _jwtService = jwtService;
            _context = context;
        }

        [BindProperty]
        public LoginModel Login { get; set; }

        public string ErrorMessage { get; set; }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.SingleOrDefault(u => u.Username == Login.Username && u.Password == Login.Password);

                if (user == null)
                {
                    ErrorMessage = "Неверное имя пользователя или пароль.";
                    return Page();
                }

                // Генерация токена
                var tokenResponse = _jwtService.GenerateToken(user);

                // Сохраняем токен в куки
                Response.Cookies.Append("jwt", tokenResponse.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = tokenResponse.Expires
                });
                
                // Здесь вы можете сохранить токен в куки или передать его клиенту как ответ.
                return LocalRedirect("/User/Projects"); // Перенаправление на главную страницу после успешного входа.
            }

            ErrorMessage = "Ошибка при входе.";
            return Page();
        }
    }
}
