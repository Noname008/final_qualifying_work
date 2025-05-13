using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace final_qualifying_work.Models
{
    public enum ProjectRole
    {
        Admin,
        User,
        Watcher
    }

    public class ProjectUser
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        [Required]
        public ProjectRole Role { get; set; }

        [Required]
        public bool Status { get; set; }
    }

    public class AddUserToProjectModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public ProjectRole Role { get; set; }
    }
}
