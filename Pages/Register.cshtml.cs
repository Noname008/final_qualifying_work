using final_qualifying_work.Data;
using final_qualifying_work.Models;
using final_qualifying_work.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace final_qualifying_work.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly JwtService _jwtService;
        private readonly AppDbContext _context;

        public RegisterModel(JwtService jwtService, AppDbContext appDbContext)
        {
            _jwtService = jwtService;
            _context = appDbContext;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Username")]
            public string Username { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var existingUser = _context.Users.SingleOrDefault(u => u.Username == Input.Username);
            if (existingUser != null)
            {
                ModelState.AddModelError("Input.Username", "Это имя пользователя уже занято");
                return Page();
            }
            // Здесь должна быть логика регистрации в базе данных
            // Для примера просто создаем пользователя

            var user = new Models.User
            {
                Username = Input.Username,
                Password = Input.Password,
                Email = Input.Email,
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            //var tokenResponse = _jwtService.GenerateToken(user);

            //// Сохраняем токен в куки
                
            //Response.Cookies.Append("jwt", tokenResponse.Token, new CookieOptions
            //{
            //    HttpOnly = true,
            //    Secure = true,
            //    SameSite = SameSiteMode.Strict,
            //    Expires = tokenResponse.Expires
            //});
            

            // Создаем аутентификационный куки
            //var claims = new List<Claim>
            //{
            //    new Claim(ClaimTypes.Name, user.Username),
            //    //new Claim(ClaimTypes.Role, user.Role),
            //    new Claim(ClaimTypes.Email, Input.Email)
            //};

            //var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            //HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(claimsIdentity)).Wait();

            return RedirectToPage("/Login");
        }
    }
    //public class RegisterModel : PageModel
    //{
    //    private readonly AppDbContext _context;

    //    public RegisterModel(AppDbContext context)
    //    {
    //        _context = context;
    //    }

    //    [BindProperty]
    //    public LoginModel Register { get; set; }

    //    public string ErrorMessage { get; set; }

    //    public async Task<IActionResult> OnPostAsync()
    //    {
    //        if (ModelState.IsValid)
    //        {
    //            //Проверка на существование пользователя
    //            var existingUser = _context.Users.SingleOrDefault(u => u.Username == Register.Username);
    //            if (existingUser != null)
    //            {
    //                ErrorMessage = "Пользователь с таким именем уже существует.";
    //                return Page();
    //            }

    //            Создание нового пользователя
    //           var user = new User
    //           {
    //               Username = Register.Username,
    //               Password = Register.Password // В реальном приложении используйте хеширование паролей!
    //           };

    //            _context.Users.Add(user);
    //            await _context.SaveChangesAsync();

    //            return RedirectToPage("/Login");
    //        }

    //        ErrorMessage = "Ошибка при регистрации.";
    //        return Page();
    //    }
    //}
}
