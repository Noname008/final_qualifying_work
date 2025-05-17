using final_qualifying_work.Models;
using final_qualifying_work.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Security.Principal;
using Task = System.Threading.Tasks.Task;

namespace final_qualifying_work.Pages.User
{
    [Authorize]
    public class ProjectsModel : PageModel
    {
        private readonly ProjectService _projectRepository;
        private readonly ILogService _logService;
        private readonly IProjectUserRepository _projectUserRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly JwtService _jwtService;

        public ProjectsModel(ProjectService projectRepository, IWebHostEnvironment environment, IProjectUserRepository projectUserRepository, ILogService logService, JwtService jwtService)
        {
            _projectRepository = projectRepository;
            _environment = environment;
            _projectUserRepository = projectUserRepository;
            _logService = logService;
            _jwtService = jwtService;
        }

        public IEnumerable<Project> Projects { get; set; }

        public async Task OnGetAsync()
        {
            Projects = await _projectRepository.GetAllPublishedProjectsAsync();
        }

        public async Task<IActionResult> OnPostCreateAsync(
            string Title,
            string Description)
        {
            try
            {
                var project = new Project
                {
                    Title = Title,
                    Description = Description,
                    CreatedDate = DateTime.Now
                };

                _projectRepository.AddProject(project);
                await _projectUserRepository.AddUserToProjectAsync(User.Claims.First(x => x.Type == ClaimValueTypes.Email).Value, project.Id, ProjectRole.Admin, true);
                await _logService.AddLog(project.Id, User.Claims.First(x => x.Type == ClaimValueTypes.Email).Value, LogProjectType.CreateProject, project.Title);

                ProjectUser ap = await _projectUserRepository.GetProjectUserAsync(project.Id, int.Parse(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value));
                string rule;
                switch (ap.Role)
                {
                    case ProjectRole.Watcher:
                        rule = "UserNone";
                        break;
                    case ProjectRole.User:
                        rule = "UserUser";
                        break;
                    case ProjectRole.Admin:
                        rule = "UserAdmin";
                        break;
                    default:
                        return LocalRedirect("/User/Projects");
                }

                var newToken = _jwtService.GenerateToken(ap.User, rule);

                Response.Cookies.Delete("jwt");
                Response.Cookies.Append("jwt", newToken.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = newToken.Expires
                });

                return LocalRedirect("/User/Projects");
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}
