using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimesheetAPI.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimesheetAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReportsController(AppDbContext context)
        {
            _context = context;
        }

        
        [HttpGet("employee/{employeeId}")]
        public async Task<IActionResult> GetMonthlyReport(int employeeId, [FromQuery] int year, [FromQuery] int month)
        {
            
            var stawkiPracownikow = new Dictionary<int, decimal>
            {
                { 1, 45.00m },  
                { 2, 50.00m },  
                { 3, 60.00m }   
            };
            decimal domyslnaStawka = 40.00m; 

            
            decimal stawkaPracownika = stawkiPracownikow.ContainsKey(employeeId)
                ? stawkiPracownikow[employeeId]
                : domyslnaStawka;


            
            var entries = await _context.TimesheetEntries
                .Include(t => t.ProjectTask)
                .Include(t => t.Employee)
                .Where(t => t.EmployeeId == employeeId
                         && t.Status == "Approved"
                         && t.Date.Year == year
                         && t.Date.Month == month)
                .ToListAsync();

            if (!entries.Any())
            {
                return NotFound(new { message = "Brak zatwierdzonych wpisów dla tego pracownika w podanym miesiącu." });
            }

            var employee = entries.First().Employee;
            string fullName = employee != null ? $"{employee.FirstName} {employee.LastName}" : "Pracownik";


            
            double totalHours = entries.Sum(e => e.Hours);
            decimal totalEarnings = (decimal)totalHours * stawkaPracownika;


            
            var taskBreakdown = entries
                .GroupBy(e => e.ProjectTask?.Name ?? "Nieznane zadanie")
                .Select(group => {
                    double hoursInTask = group.Sum(e => e.Hours);
                    return new
                    {
                        TaskName = group.Key,
                        HoursSpent = hoursInTask,
                        EarningsInTask = (decimal)hoursInTask * stawkaPracownika
                    };
                })
                .ToList();


            
            return Ok(new
            {
                EmployeeName = fullName,
                Period = $"{month:D2}/{year}",
                TotalHours = totalHours,
                TotalEarnings = totalEarnings,
                TasksDetail = taskBreakdown
            });
        }
    }
}