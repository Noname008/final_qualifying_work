using final_qualifying_work.Models;
using final_qualifying_work.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace final_qualifying_work.Pages.User
{
    [Authorize]
    public class TasksModel : PageModel
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ProjectService _projectRepository;

        public TasksModel(ITaskRepository taskRepository, ProjectService projectRepository)
        {
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
        }

        public Project Project { get; set; }
        public List<ProjectTask> TodoTasks { get; set; } = new List<ProjectTask>();
        public List<ProjectTask> InProgressTasks { get; set; } = new List<ProjectTask>();
        public List<ProjectTask> DoneTasks { get; set; } = new List<ProjectTask>();

        public async Task<IActionResult> OnGetAsync(int projectId)
        {
            Project = await _projectRepository.GetProjectByIdAsync(projectId);
            if (Project == null)
            {
                return NotFound();
            }
            var allTasks = await _taskRepository.GetTasksByProjectAsync(projectId);

            TodoTasks = allTasks.Where(t => t.Status == Models.TaskStatus.ToDo).ToList();
            InProgressTasks = allTasks.Where(t => t.Status == Models.TaskStatus.InProgress).ToList();
            DoneTasks = allTasks.Where(t => t.Status == Models.TaskStatus.Done).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostMoveTaskAsync([FromBody] ProjectTask projectTask)
        {
            await _taskRepository.MoveTaskAsync(projectTask.Id, projectTask.Status);
            return new JsonResult(new { success = true });
        }

        public async Task<IActionResult> OnPostAddTaskAsync([FromBody] ProjectTask projectTask)
        {
            try
            {
                await _taskRepository.AddTaskAsync(projectTask);

                return new JsonResult(new
                {
                    success = true,
                    taskId = projectTask.Id,
                    createdDate = projectTask.CreatedDate.ToString("g")
                });

            }catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }

        }
    }
}
