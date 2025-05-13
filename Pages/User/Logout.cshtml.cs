using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace final_qualifying_work.Pages.User
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            Response.Cookies.Delete("jwt");
            return RedirectToPage("/Login");
        }
        public IActionResult OnPost()
        {
            Response.Cookies.Delete("jwt");
            return RedirectToPage("/Login");
        }
    }
}
