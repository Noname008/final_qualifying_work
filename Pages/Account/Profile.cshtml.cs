using final_qualifying_work.Data;
using final_qualifying_work.Models;
using final_qualifying_work.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace final_qualifying_work.Pages.Account
{
    [Authorize]
    public class ProfileModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;
        private readonly IUserService _userService;

        public ProfileModel(
            AppDbContext context,
            JwtService jwtService,
            IUserService userService)
        {
            _context = context;
            _jwtService = jwtService;
            _userService = userService;
        }

        [BindProperty]
        public UserProfileModel Input { get; set; }

        public string StatusMessage { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; }

        public async Task OnGetAsync(string message = null, bool isSuccess = false)
        {
            Message = message;
            IsSuccess = isSuccess;

            var userId = Int32.Parse(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
            var user = await _userService.GetUserByIdAsync(userId);

            if (user != null)
            {
                Input = new UserProfileModel
                {
                    Username = user.Username,
                    Email = user.Email
                };
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = Int32.Parse(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var result = await _userService.UpdateUserProfileAsync(userId, Input);

            if (result.Success)
            {
                // Генерация нового токена при изменении email/username
                var newToken = _jwtService.GenerateToken(result.User);
                Response.Cookies.Append("jwt", newToken.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = newToken.Expires
                });

                return RedirectToPage(new { message = "Профиль успешно обновлен", isSuccess = true });
            }
            ModelState.AddModelError(string.Empty, result.Message);

            return Page();
        }
    }
}
