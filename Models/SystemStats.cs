namespace final_qualifying_work.Models
{
    public class SystemStats
    {
        public int TotalUsers { get; set; } = 0;
        public int ActiveUsersToday { get; set; } = 0;
        public int NewUsersThisWeek { get; set; } = 0;

        public int TotalProjects { get; set; } = 0;
        public int ActiveProjects { get; set; } = 0;

        public int TotalMeetings { get; set; } = 0;
        public int MeetingsToday { get; set; } = 0;

        public double CpuUsage { get; set; } = 0; // %
        public double MemoryUsage { get; set; } = 0; // %
        public double DiskUsage { get; set; } = 0; // %

        public DateTime SystemUptime { get; set; }
        public DateTime LastBackup { get; set; }

        public int PendingTasks { get; set; } = 0;
        public int CompletedTasksToday { get; set; } = 0;

        public int UnreadMessages { get; set; }
        public int OpenSupportTickets { get; set; }

        public DatabaseStats Database { get; set; }
    }

    public class DatabaseStats
    {
        public int TotalTables { get; set; }
        public long TotalSizeMB { get; set; }
        public int DailyGrowthMB { get; set; }
    }
}
