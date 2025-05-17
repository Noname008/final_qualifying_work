using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace final_qualifying_work.Models
{
    public class LogProject
    {
        public int Id { get; set; }

        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        [ForeignKey("User")]
        public int userId { get; set; }
        public User? User { get; set; }

        public LogProjectType LogType { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public string Description { get; set; }
    }

    public enum LogProjectType
    {
        CreateProject = 0,
        CreateTask = 1,
        UpdateTask = 2,
        UpdateStatusTask = 3,
        DeleteTask = 4,
        AddUser = 5,
        DeleteUser = 6,
        AddMeeting = 7,
        UpdateMeeting = 8,
        DeleteMeeting = 9,
        UserAccepted = 10,
        UserRejected = 11,
    }
}
