using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace final_qualifying_work.Models
{
    public class ProjectTask
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        public string Description { get; set; }

        public string TimeExec { get; set; }

        public string Category { get; set; }

        public string Skills { get; set; }

        [Required]
        public TaskStatus Status { get; set; } = TaskStatus.ToDo;

        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }

        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        [ForeignKey("User")]
        public string? AssigneeId { get; set; }
        public User? Assignee { get; set; }
    }

    public enum TaskStatus
    {
        ToDo = 0,
        InProgress = 1,
        Done = 2,
        Delete = 3
    }
}
