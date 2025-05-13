using final_qualifying_work.Data;
using final_qualifying_work.Models;
using Microsoft.EntityFrameworkCore;

namespace final_qualifying_work.Services
{
    public class ProjectService
    {
        private readonly AppDbContext _context;

        public ProjectService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Project>> GetAllPublishedProjectsAsync()
        {
            return await _context.Projects.ToListAsync();
        }

        public async Task<Project> GetProjectByIdAsync(int id)
        {
            return await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public void DeleteProjectAsync(Project project)
        {
            _context.Projects.Remove(project);
            _context.SaveChanges();
        }

        public void AddProject(Project project)
        {
            _context.Projects.Add(project);
            _context.SaveChanges();
        }

    }
}
