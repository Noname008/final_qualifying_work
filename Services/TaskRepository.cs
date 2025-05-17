using final_qualifying_work.Data;
using final_qualifying_work.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace final_qualifying_work.Services
{
    public interface ITaskRepository
    {
        Task<IEnumerable<ProjectTask>> GetTasksByProjectAsync(int projectId);
        Task<ProjectTask> GetTaskByIdAsync(int taskId);
        Task AddTaskAsync(ProjectTask task);
        Task UpdateTaskAsync(ProjectTask task);
        Task DeleteTaskAsync(int taskId);
        Task MoveTaskAsync(int taskId, Models.TaskStatus newStatus);
    }

    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _context;
        private readonly AiService _aiService;

        public TaskRepository(AppDbContext context, AiService aiService)
        {
            _context = context;
            _aiService = aiService;
        }

        public async Task<IEnumerable<ProjectTask>> GetTasksByProjectAsync(int projectId)
        {
            return await _context.Tasks
                .Where(t => t.ProjectId == projectId)
                .Include(t => t.Assignee)
                .OrderBy(t => t.Status)
                .ThenByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

        public async Task<ProjectTask> GetTaskByIdAsync(int taskId)
        {
            return await _context.Tasks
                .FirstAsync(t => t.Id == taskId);
        }

        public async Task AddTaskAsync(ProjectTask task)
        {
            var t = await _aiService.Task(task.Description);
            task.TimeExec = t.time;
            task.Category = t.category;
            task.Skills = t.skills.Length <= 3
                ? string.Join(", ", t.skills)
                : string.Join(", ", t.skills.Take(3)) + "...";
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTaskAsync(ProjectTask task)
        {
            Console.WriteLine(task.Id);
            var t = await GetTaskByIdAsync(task.Id);
            t.Title = task.Title;
            t.Description = task.Description;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTaskAsync(int taskId)
        {
            var task = await _context.Tasks.FindAsync(taskId);
            if (task != null)
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
            }
        }

        public async Task MoveTaskAsync(int taskId, Models.TaskStatus newStatus)
        {
            var task = await _context.Tasks.FindAsync(taskId);
            if (task != null)
            {
                task.Status = newStatus;
                await _context.SaveChangesAsync();
            }
        }
    }
}
