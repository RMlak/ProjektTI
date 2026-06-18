using Microsoft.EntityFrameworkCore;
using TimesheetAPI.Models;

namespace TimesheetAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        
        public DbSet<Employee> Employees { get; set; }
        public DbSet<ProjectTask> ProjectTasks { get; set; }
        public DbSet<TimesheetEntry> TimesheetEntries { get; set; }
        public DbSet<EmployeeProjectTask> EmployeeProjectTasks { get; set; }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EmployeeProjectTask>()
                .HasKey(et => new { et.EmployeeId, et.ProjectTaskId });

            modelBuilder.Entity<EmployeeProjectTask>()
                .HasOne(et => et.Employee)
                .WithMany()
                .HasForeignKey(et => et.EmployeeId);

            modelBuilder.Entity<EmployeeProjectTask>()
                .HasOne(et => et.ProjectTask)
                .WithMany()
                .HasForeignKey(et => et.ProjectTaskId);
        }
    }
}