using final_qualifying_work.Models;
using final_qualifying_work.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace final_qualifying_work.Pages.Account
{
    [Authorize]
    public class InvitationsModel : PageModel
    {
        private readonly IProjectUserRepository _invitationRepo;

        public InvitationsModel(
            IProjectUserRepository invitationRepo)
        {
            _invitationRepo = invitationRepo;
        }

        public List<ProjectUser> Invitations { get; set; } = [];

        public async Task OnGetAsync()
        {
            Invitations = (await _invitationRepo.GetProjectsByUserAsync(User.Claims.First(x => x.Type == ClaimValueTypes.Email).Value)).Where(x => x.Status == false).ToList();
        }

        public async Task<IActionResult> OnPostAcceptAsync(int id)
        {
            await _invitationRepo.AcceptedUserFromProjectAsync(id);

            TempData["Message"] = "Приглашение принято! Теперь вы участник проекта.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeclineAsync(int id)
        {
            await _invitationRepo.DeletedUserFromProjectAsync(id);

            TempData["Message"] = "Приглашение отклонено.";
            return RedirectToPage();
        }
    }
}
