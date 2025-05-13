using final_qualifying_work.Models;
using final_qualifying_work.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics;

namespace final_qualifying_work.Pages.Admin
{
    //[Authorize(Roles = "Admin")]
    public class DashboardModel : PageModel
    {
        private readonly IStatsService _statsService;
        private readonly HealthCheckService _healthCheckService;
        private readonly ILogger<DashboardModel> _logger;

        public DashboardModel(
            IStatsService statsService,
            HealthCheckService healthCheckService,
            ILogger<DashboardModel> logger)
        {
            _statsService = statsService;
            _healthCheckService = healthCheckService;
            _logger = logger;
        }

        public SystemStats Stats { get; set; }
        public List<SystemLog> RecentLogs { get; set; }
        public List<string> UserActivityLabels { get; set; }
        public List<int> UserActivityData { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                Stats = await _statsService.GetSystemStatsAsync();
                await LoadHealthMetrics();
                LoadSampleLogs();
                GenerateActivityData();

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке админ панели");
                return RedirectToPage("/Error");
            }
        }

        public async Task<IActionResult> OnGetStatsAsync()
        {
            var stats = await _statsService.GetSystemStatsAsync();
            await LoadHealthMetrics(stats);

            return new JsonResult(new
            {
                cpuUsage = stats.CpuUsage,
                memoryUsage = stats.MemoryUsage,
                diskUsage = stats.DiskUsage,
                userActivityData = UserActivityData
            });
        }

        public async Task<IActionResult> OnPostCreateBackupAsync()
        {
            try
            {
                // Здесь должна быть логика создания бэкапа
                // Например, вызов внешнего сервиса или команды

                Stats.LastBackup = DateTime.Now;
                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании бэкапа");
                return new JsonResult(new { success = false, error = ex.Message });
            }
        }

        private async Task LoadHealthMetrics(SystemStats stats = null)
        {
            stats ??= Stats;

            try
            {
                var healthReport = await _healthCheckService.CheckHealthAsync();

                stats.CpuUsage = GetRandomValue(10, 70); // Замените реальными данными
                stats.MemoryUsage = GetRandomValue(30, 80);
                stats.DiskUsage = GetRandomValue(20, 60);
                stats.SystemUptime = Process.GetCurrentProcess().StartTime;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке метрик здоровья");
                stats.CpuUsage = 0;
                stats.MemoryUsage = 0;
                stats.DiskUsage = 0;
            }
        }

        private void LoadSampleLogs()
        {
            RecentLogs = new List<SystemLog>
            {
                new SystemLog
                {
                    Timestamp = DateTime.Now.AddMinutes(-5),
                    EventType = "UserLogin",
                    Username = User.Identity.Name,
                    Message = "Успешный вход в систему"
                },
                new SystemLog
                {
                    Timestamp = DateTime.Now.AddMinutes(-15),
                    EventType = "ProjectCreated",
                    Username = "admin@example.com",
                    Message = "Создан новый проект 'Разработка API'"
                },
                new SystemLog
                {
                    Timestamp = DateTime.Now.AddMinutes(-30),
                    EventType = "MeetingScheduled",
                    Username = "user@example.com",
                    Message = "Запланировано совещание на 15:00"
                },
                new SystemLog
                {
                    Timestamp = DateTime.Now.AddHours(-1),
                    EventType = "System",
                    Username = "SYSTEM",
                    Message = "Выполнено ежедневное обслуживание"
                }
            };
        }

        private void GenerateActivityData()
        {
            UserActivityLabels = new List<string>();
            UserActivityData = new List<int>();

            var rnd = new Random();
            for (int i = 6; i >= 0; i--)
            {
                var date = DateTime.Today.AddDays(-i);
                UserActivityLabels.Add(date.ToString("dd MMM"));
                UserActivityData.Add(rnd.Next(5, 20));
            }
        }

        private double GetRandomValue(int min, int max)
        {
            var rnd = new Random();
            return rnd.Next(min, max) + rnd.NextDouble();
        }
    }

    public class SystemLog
    {
        public DateTime Timestamp { get; set; }
        public string EventType { get; set; }
        public string Username { get; set; }
        public string Message { get; set; }
    }
}
