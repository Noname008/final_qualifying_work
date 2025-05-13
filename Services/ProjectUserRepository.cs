using final_qualifying_work.Data;
using final_qualifying_work.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace final_qualifying_work.Services
{
    public interface IProjectUserRepository
    {
        Task<List<ProjectUser>> GetUsersByProjectAsync(int projectId);
        Task<ProjectUser> GetProjectUserAsync(int projectId, int userId);
        Task AddUserToProjectAsync(string email, int projectId, ProjectRole role, bool status = false);
        Task UpdateUserRoleAsync(int projectId, int userId, ProjectRole newRole);
        Task RemoveUserFromProjectAsync(int projectId, int userId);
        Task<List<ProjectUser>> GetProjectsByUserAsync(string email);
        Task AcceptedUserFromProjectAsync(int id);
        Task DeletedUserFromProjectAsync(int id);
    }

    public class ProjectUserRepository : IProjectUserRepository
    {
        private readonly AppDbContext _context;

        public ProjectUserRepository(
            AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProjectUser>> GetUsersByProjectAsync(int projectId)
        {
            return await _context.ProjectUsers
                .Include(pu => pu.User)
                .Where(pu => pu.ProjectId == projectId && pu.Status == true)
                .OrderByDescending(pu => pu.Role)
                .ThenBy(pu => pu.User.Email)
                .ToListAsync();
        }

        public async Task<ProjectUser> GetProjectUserAsync(int projectId, int userId)
        {
            return await _context.ProjectUsers
                .FirstOrDefaultAsync(pu => pu.ProjectId == projectId && pu.UserId == userId);
        }

        public async Task AddUserToProjectAsync(string email, int projectId, ProjectRole role, bool status = false)
        {
            var user = await _context.Users.FirstOrDefaultAsync(pu => pu.Email == email);
            if (user == null)
            {
                throw new System.Exception("Пользователь с таким email не найден");
            }

            var existing = await _context.ProjectUsers
                .AnyAsync(pu => pu.ProjectId == projectId && pu.UserId == user.Id);
            
            if (existing)
            {
                throw new System.Exception("Пользователь уже добавлен в проект");
            }

            var projectUser = new ProjectUser
            {
                UserId = user.Id,
                ProjectId = projectId,
                Role = role,
                Status = status
            };

            _context.ProjectUsers.Add(projectUser);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserRoleAsync(int projectId, int userId, ProjectRole newRole)
        {
            var projectUser = await GetProjectUserAsync(projectId, userId);
            if (projectUser == null)
            {
                throw new System.Exception("Пользователь не найден в проекте");
            }

            projectUser.Role = newRole;
            await _context.SaveChangesAsync();
        }

        public async Task RemoveUserFromProjectAsync(int projectId, int userId)
        {
            var projectUser = await GetProjectUserAsync(projectId, userId);
            if (projectUser != null)
            {
                _context.ProjectUsers.Remove(projectUser);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<ProjectUser>> GetProjectsByUserAsync(string email)
        {
            User user = await _context.Users.FirstOrDefaultAsync(pu => pu.Email == email);
            if (user == null)
            {
                throw new System.Exception("Пользователь с таким email не найден");
            }
            return await _context.ProjectUsers
                .Include(pu => pu.Project)
                .Where(pu => pu.UserId == user.Id)
                .OrderByDescending(pu => pu.Role)
                .ThenBy(pu => pu.User.Email)
                .ToListAsync();
        }

        public async Task AcceptedUserFromProjectAsync(int id)
        {
            _context.ProjectUsers.First(x => x.Id == id).Status = true;
            await _context.SaveChangesAsync();
        }

        public async Task DeletedUserFromProjectAsync(int id)
        {
            _context.ProjectUsers.Remove(_context.ProjectUsers.First(x => x.Id == id));
            await _context.SaveChangesAsync();
        }
    }
}
