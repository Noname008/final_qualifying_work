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
        private readonly JwtService _jwtService;
        private readonly IUserService _userService;

        public string Token { get; private set; }

        public LoginPageModel(JwtService jwtService, IUserService userService)
        {
            _jwtService = jwtService;
            _userService = userService;
        }

        [BindProperty]
        public LoginModel Login { get; set; }

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = new Models.User
                {
                    Username = Login.Username,
                    Password = Login.Password,
                };

                var mess = await _userService.VerifyUserAsync(user);

                if (!mess.Success)
                {
                    ErrorMessage = "�������� ��� ������������ ��� ������.";
                    return Page();
                }

                // ��������� ������
                var tokenResponse = _jwtService.GenerateToken(mess.User);

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
