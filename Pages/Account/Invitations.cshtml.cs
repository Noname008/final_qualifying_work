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
        private readonly ILogService _logService;

        public InvitationsModel(IProjectUserRepository invitationRepo, ILogService logService)
        {
            _invitationRepo = invitationRepo;
            _logService = logService;
        }

        public List<ProjectUser> Invitations { get; set; } = [];

        public async Task OnGetAsync()
        {
            Invitations = (await _invitationRepo.GetProjectsByUserAsync(User.Claims.First(x => x.Type == ClaimValueTypes.Email).Value)).Where(x => x.Status == false).ToList();
        }

        public async Task<IActionResult> OnPostAcceptAsync(int id)
        {
            await _invitationRepo.AcceptedUserFromProjectAsync(id);
            await _logService.AddLog(id, User.Claims.First(x => x.Type == ClaimValueTypes.Email).Value, LogProjectType.UserAccepted, "");

            TempData["Message"] = "Приглашение принято! Теперь вы участник проекта.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeclineAsync(int id)
        {
            await _invitationRepo.DeletedUserFromProjectAsync(id);
            await _logService.AddLog(id, User.Claims.First(x => x.Type == ClaimValueTypes.Email).Value, LogProjectType.UserRejected, "");

            TempData["Message"] = "Приглашение отклонено.";
            return RedirectToPage();
        }
    }
}
