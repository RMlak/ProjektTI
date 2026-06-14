using Microsoft.EntityFrameworkCore;
using TimesheetAPI.Models;

namespace TimesheetAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<TimesheetEntry> TimesheetEntries { get; set; }
        // Ta linia jest nowa:
        public DbSet<ProjectTask> ProjectTasks { get; set; }
    }
}