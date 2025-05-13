using final_qualifying_work.Models;
using Microsoft.EntityFrameworkCore;

namespace final_qualifying_work.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Project> Projects { get; set; }

        public DbSet<ProjectTask> Tasks { get; set; }

        public DbSet<ProjectUser> ProjectUsers { get; set; }

        public DbSet<Meeting> Meetings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProjectTask>()
                .Property(t => t.Status)
                .HasConversion<string>();
            modelBuilder.Entity<ProjectUser>()
                .Property(pu => pu.Role)
                .HasConversion<string>();
        }
    }
}
