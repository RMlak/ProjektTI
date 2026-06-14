using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimesheetAPI.Data;

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

        // GET: api/Reports/employee/{employeeId}?year=2026&month=6
        // Generuje pełny raport miesięczny: sumuje zatwierdzone godziny i wylicza pensję
        [HttpGet("employee/{employeeId}")]
        public async Task<IActionResult> GetMonthlyReport(int employeeId, [FromQuery] int year, [FromQuery] int month)
        {
            // 1. Wyciągamy z bazy tylko ZATWIERDZONE wpisy danego pracownika z wybranego miesiąca
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

            // 2. MATEMATYKA: Sumujemy godziny i wyliczamy zarobki (Godziny * Stawka danego zadania)
            double totalHours = entries.Sum(e => e.Hours);
            decimal totalEarnings = entries.Sum(e => (decimal)e.Hours * (e.ProjectTask?.HourlyRate ?? 0));

            // 3. STATYSTYKA: Grupowanie danych, żeby pokazać ile czasu zeszło na konkretne zadania
            var taskBreakdown = entries
                .GroupBy(e => e.ProjectTask?.Name ?? "Nieznane zadanie")
                .Select(group => new
                {
                    TaskName = group.Key,
                    HoursSpent = group.Sum(e => e.Hours),
                    EarningsInTask = group.Sum(e => (decimal)e.Hours * (group.First().ProjectTask?.HourlyRate ?? 0))
                })
                .ToList();

            // 4. Zwracamy gotowy, czytelny raport finansowy
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