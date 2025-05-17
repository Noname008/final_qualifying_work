using final_qualifying_work.Data;
using final_qualifying_work.Models;
using Microsoft.EntityFrameworkCore;

namespace final_qualifying_work.Services
{
    public interface ILogService
    {
        Task AddLog(int projectId, string email, LogProjectType type, string addNameDescription);
        IQueryable<LogProject> GetLogs(int projectId);
    }
    public class LogService : ILogService
    {
        private readonly AppDbContext _context;

        public LogService(AppDbContext context)
        {
            _context = context; 
        }

        public async Task AddLog(int projectId, string email, LogProjectType type, string addNameDescription)
        {
            var user = await _context.Users.FirstOrDefaultAsync(pu => pu.Email == email);
            if (user == null)
            {
                throw new System.Exception("Пользователь с таким email не найден");
            }

            string Description = "Пользователь " + user.Username + getDescription(type) + addNameDescription + ".";

            LogProject log = new LogProject()
            {
                CreatedDate = DateTime.Now,
                Description = Description,
                LogType = type,
                ProjectId = projectId,
                userId = user.Id,
            };
            _context.logProjects.Add(log);
            await _context.SaveChangesAsync();
        }

        public IQueryable<LogProject> GetLogs(int projectId)
        {
            return _context.logProjects
                .Include(l => l.Project)
                .Include(l => l.User)
                .Where(l => l.ProjectId == projectId);
        }

        private string getDescription(LogProjectType type)
        {
            switch (type)
            {
                case LogProjectType.CreateProject:
                    return " создал проект: ";
                case LogProjectType.CreateTask:
                    return " создал задачу: ";
                case LogProjectType.UpdateTask:
                    return " изменил задачу: ";
                case LogProjectType.UpdateStatusTask:
                    return " изменил статус задачи: ";
                case LogProjectType.DeleteTask:
                    return " удалил задачу: ";
                case LogProjectType.AddUser:
                    return " отправил запрос на добавление пользователю: ";
                case LogProjectType.DeleteUser:
                    return " удалил пользователя: ";
                case LogProjectType.AddMeeting:
                    return " создал встречу: ";
                case LogProjectType.UpdateMeeting:
                    return " изменил встречу: ";
                case LogProjectType.DeleteMeeting:
                    return " удалил встречу: ";
                case LogProjectType.UserAccepted:
                    return " принял запрос на добавление в проект";
                case LogProjectType.UserRejected:
                    return " отклонил запрос на добавление в проект";
                default:
                    return "";
            }
        }
    }
}
