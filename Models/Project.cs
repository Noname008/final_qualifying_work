using System.ComponentModel.DataAnnotations;

namespace final_qualifying_work.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public required string Title { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; }

        public ICollection<ProjectTask> Tasks { get; set; }
    }
}
