using final_qualifying_work.Data;
using final_qualifying_work.Models;
using Microsoft.EntityFrameworkCore;

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

        public TaskRepository(AppDbContext context)
        {
            _context = context;
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
                .Include(t => t.Assignee)
                .FirstOrDefaultAsync(t => t.Id == taskId);
        }

        public async Task AddTaskAsync(ProjectTask task)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTaskAsync(ProjectTask task)
        {
            _context.Tasks.Update(task);
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
