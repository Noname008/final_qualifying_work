using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace final_qualifying_work.Models
{
    public class Meeting
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public string Location { get; set; }

        [StringLength(50)]
        public string OrganizerId { get; set; }

        public bool IsOnline { get; set; }

        public string MeetingUrl { get; set; }

        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public string Participants { get; set; } // JSON массив email'ов
    }
}
