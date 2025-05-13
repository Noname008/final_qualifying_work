using final_qualifying_work.Data;
using final_qualifying_work.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace final_qualifying_work.Services
{
    public interface IStatsService
    {
        Task<SystemStats> GetSystemStatsAsync();
        Task<DatabaseStats> GetDatabaseStatsAsync();
    }

    public class StatsService : IStatsService
    {
        private readonly AppDbContext _context;
        private readonly IMeetingRepository _meetingRepo;
        private readonly ProjectService _projectRepo;

        public StatsService(
            AppDbContext context,
            IMeetingRepository meetingRepo,
            ProjectService projectRepo)
        {
            _context = context;
            _meetingRepo = meetingRepo;
            _projectRepo = projectRepo;
        }

        public async Task<SystemStats> GetSystemStatsAsync()
        {
            var today = DateTime.Today;
            var weekAgo = today.AddDays(-7);

            var stats = new SystemStats
            {
                // Пользователи
                //TotalUsers = _userManager.Users.Count(),
                //ActiveUsersToday = await _context.UserActivities
                //    .Where(a => a.ActivityDate >= today)
                //    .Select(a => a.UserId)
                //    .Distinct()
                //    .CountAsync(),
                //NewUsersThisWeek = await _userManager.Users
                //    .Where(u => u.CreatedDate >= weekAgo)
                //    .CountAsync(),

                //// Проекты
                //TotalProjects = await _projectRepo.GetCountAsync(),
                //ActiveProjects = await _projectRepo.GetActiveCountAsync(),

                //// Совещания
                //TotalMeetings = await _meetingRepo.GetCountAsync(),
                //MeetingsToday = await _meetingRepo.GetCountByDateAsync(today),

                //// Системные метрики (заполняются отдельно)

                //// Задачи
                //PendingTasks = await _context.Tasks
                //    .Where(t => !t.IsCompleted)
                //    .CountAsync(),
                //CompletedTasksToday = await _context.Tasks
                //    .Where(t => t.CompletedDate >= today)
                //    .CountAsync(),

                //// Сообщения и тикеты
                //UnreadMessages = await _context.Messages
                //    .Where(m => !m.IsRead)
                //    .CountAsync(),
                //OpenSupportTickets = await _context.SupportTickets
                //    .Where(t => !t.IsClosed)
                //    .CountAsync(),

                //Database = await GetDatabaseStatsAsync()
            };

            return stats;
        }

        public async Task<DatabaseStats> GetDatabaseStatsAsync()
        {
            // Пример для SQL Server
            var connection = _context.Database.GetDbConnection();
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT 
                    COUNT(*) as TotalTables,
                    SUM(size * 8.0 / 1024) as TotalSizeMB
                FROM sys.tables t
                JOIN sys.indexes i ON t.object_id = i.object_id
                JOIN sys.partitions p ON i.object_id = p.object_id AND i.index_id = p.index_id
                JOIN sys.allocation_units a ON p.partition_id = a.container_id
                GROUP BY t.name";

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new DatabaseStats
                {
                    TotalTables = reader.GetInt32(0),
                    TotalSizeMB = reader.GetInt64(1),
                    DailyGrowthMB = await CalculateDailyGrowth()
                };
            }

            return new DatabaseStats();
        }

        private async Task<int> CalculateDailyGrowth()
        {
            // Логика расчета ежедневного роста БД
            return 0; // Замените реальной логикой
        }
    }
}
