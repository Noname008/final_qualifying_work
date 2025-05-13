using final_qualifying_work.Data;
using final_qualifying_work.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace final_qualifying_work.Pages.User
{
    [Authorize]
    public class PrivacyModel : PageModel
    {
        private readonly AppDbContext _context;

        public PrivacyModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Product> Users { get; set; }

        public void OnGet()
        {
            // Извлечение всех пользователей из базы данных
            Users = _context.Products.ToList();
        }
    }

}
