using Microsoft.EntityFrameworkCore;
using TimesheetAPI.Models;

namespace TimesheetAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Te właściwości mówią EF Core, jakie tabele mają powstać w bazie
    public DbSet<Employee> Employees { get; set; }
    public DbSet<TimesheetEntry> TimesheetEntries { get; set; }
}