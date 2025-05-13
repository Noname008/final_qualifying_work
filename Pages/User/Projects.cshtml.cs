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
        private readonly IProjectUserRepository _projectUserRepository;
        private readonly IWebHostEnvironment _environment;

        public ProjectsModel(ProjectService projectRepository, IWebHostEnvironment environment, IProjectUserRepository projectUserRepository)
        {
            _projectRepository = projectRepository;
            _environment = environment;
            _projectUserRepository = projectUserRepository;
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

                return LocalRedirect("/User/Projects");
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}
