using final_qualifying_work.Data;
using final_qualifying_work.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;

namespace final_qualifying_work.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public LoginModel Input { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet()
        {
            // ������������� ������ ��� GET �������, ���� �����.
            Input = new LoginModel();
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                // ����� �� ������ �������� ������ �������� ������������.
                // ��������, �������� ����� ������������ � ������.
                if (Input.Username == "1" && Input.Password == "1") // ������ ��������
                {
                    _context.Add(new Models.User { Id = 0, Username = "2", Password = "2" });
                    _context.SaveChangesAsync().Wait();
                    return RedirectToPage("/Index"); // ��������������� �� ������� �������� ����� ��������� �����.

                }
                else
                {
                    ErrorMessage = "�������� ��� ������������ ��� ������.";
                }
            }

            return Page(); // ���������� �� �� �������� � �������.
        }
    }
}
