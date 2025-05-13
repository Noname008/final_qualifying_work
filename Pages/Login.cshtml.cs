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
                    ErrorMessage = "�������� ��� ������������ ��� ������.";
                    return Page();
                }

                // ��������� ������
                var tokenResponse = _jwtService.GenerateToken(user);

                // ��������� ����� � ����
                Response.Cookies.Append("jwt", tokenResponse.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = tokenResponse.Expires
                });
                
                // ����� �� ������ ��������� ����� � ���� ��� �������� ��� ������� ��� �����.
                return LocalRedirect("/User/Projects"); // ��������������� �� ������� �������� ����� ��������� �����.
            }

            ErrorMessage = "������ ��� �����.";
            return Page();
        }
    }
}
