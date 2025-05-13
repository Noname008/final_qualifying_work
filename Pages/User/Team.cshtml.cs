using final_qualifying_work.Models;
using final_qualifying_work.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace final_qualifying_work.Pages.User
{
    [Authorize]
    public class TeamModel : PageModel
    {
        private readonly IProjectUserRepository _projectUserRepository;
        private readonly ProjectService _projectRepository;

        public TeamModel(
            IProjectUserRepository projectUserRepository,
            ProjectService projectRepository)
        {
            _projectUserRepository = projectUserRepository;
            _projectRepository = projectRepository;
        }

        public Project Project { get; set; }
        public List<ProjectUser> ProjectUsers { get; set; } = new List<ProjectUser>();

        public async Task<IActionResult> OnGetAsync(int projectId)
        {
            Project = await _projectRepository.GetProjectByIdAsync(projectId);
            if (Project == null)
            {
                return NotFound();
            }

            ProjectUsers = await _projectUserRepository.GetUsersByProjectAsync(projectId);
            return Page();
        }

        public async Task<IActionResult> OnPostUpdateRoleAsync([FromBody] UserModel model)
        {
            try
            {
                if (!Enum.TryParse(model.Role, out ProjectRole role))
                {
                    return new JsonResult(new { success = false, error = "Некорректная роль" });
                }

                await _projectUserRepository.UpdateUserRoleAsync(model.projectId, model.userId, role);
                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, error = ex.Message });
            }
        }

        public async Task<IActionResult> OnPostRemoveUserAsync([FromBody] UserModel model)
        {
            try
            {
                await _projectUserRepository.RemoveUserFromProjectAsync(model.projectId, model.userId);
                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, error = ex.Message });
            }
        }

        public async Task<IActionResult> OnPostAddUserAsync([FromBody] UserModel model)
        {
            try
            {
                if (!Enum.TryParse(model.Role, out ProjectRole projectRole))
                {
                    return new JsonResult(new { success = false, error = "Некорректная роль" });
                }

                await _projectUserRepository.AddUserToProjectAsync(model.Email, model.projectId, projectRole);
                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, error = ex.Message });
            }
        }
    }
    public class UserModel
    {
        public int userId { get; set; }
        public int projectId { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
