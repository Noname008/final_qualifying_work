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
        private readonly IUserService _userService;

        public RegisterModel(JwtService jwtService, IUserService userService)
        {
            _jwtService = jwtService;
            _userService = userService;
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
            var user = new Models.User
            {
                Username = Input.Username,
                Email = Input.Email,
                Password = Input.Password,
            };
            var mess = await _userService.RegistryUserAsync(user);

            if (!mess.Success)
            {
                ModelState.AddModelError("Input.Username", mess.Message);
                return Page();
            }

            return RedirectToPage("/Login");
        }
    }
}
