namespace final_qualifying_work.Models
{
    // ProjectChat.cs
    public class ProjectChat
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public DateTime CreatedAt { get; set; }

        // Навигационные свойства
        public Project Project { get; set; }
        public ICollection<ProjectMessage> Messages { get; set; }
    }

    // ProjectMessage.cs
    public class ProjectMessage
    {
        public int Id { get; set; }
        public int ChatId { get; set; }
        public int UserId { get; set; }
        public string MessageText { get; set; }
        public DateTime SentAt { get; set; }

        // Навигационные свойства
        public ProjectChat Chat { get; set; }
        public User User { get; set; }
    }
}
