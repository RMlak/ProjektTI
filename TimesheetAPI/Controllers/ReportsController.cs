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

        // GET: api/Reports/employee/{employeeId}?year=2026&month=6
        // Generuje pełny raport miesięczny: sumuje zatwierdzone godziny i wylicza pensję na podstawie odgórnej stawki
        [HttpGet("employee/{employeeId}")]
        public async Task<IActionResult> GetMonthlyReport(int employeeId, [FromQuery] int year, [FromQuery] int month)
        {
            // 1. SŁOWNIK ODGÓRNYCH STAWEK (ID pracownika : stawka za godzinę)
            // Możesz tutaj swobodnie zmieniać i dodawać pracowników
            var stawkiPracownikow = new Dictionary<int, decimal>
            {
                { 1, 45.00m },  // Pracownik o ID 1 zarabia 45.00 zł/h
                { 2, 50.00m },  // Pracownik o ID 2 zarabia 50.00 zł/h
                { 3, 60.00m }   // Pracownik o ID 3 zarabia 60.00 zł/h
            };
            decimal domyslnaStawka = 40.00m; // Stawka dla pozostałych pracowników, których nie ma w słowniku

            // Pobieramy stawkę dla aktualnie sprawdzanego pracownika
            decimal stawkaPracownika = stawkiPracownikow.ContainsKey(employeeId)
                ? stawkiPracownikow[employeeId]
                : domyslnaStawka;


            // 2. Wyciągamy z bazy tylko ZATWIERDZONE wpisy danego pracownika z wybranego miesiąca
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


            // 3. MATEMATYKA: Sumujemy godziny i wyliczamy zarobki (Godziny * Odgórna stawka pracownika)
            double totalHours = entries.Sum(e => e.Hours);
            decimal totalEarnings = (decimal)totalHours * stawkaPracownika;


            // 4. STATYSTYKA: Grupowanie danych, żeby pokazać ile czasu zeszło na konkretne zadania
            var taskBreakdown = entries
                .GroupBy(e => e.ProjectTask?.Name ?? "Nieznane zadanie")
                .Select(group => {
                    double hoursInTask = group.Sum(e => e.Hours);
                    return new
                    {
                        TaskName = group.Key,
                        HoursSpent = hoursInTask,
                        // Tutaj też mnożymy przez odgórną stawkę pracownika
                        EarningsInTask = (decimal)hoursInTask * stawkaPracownika
                    };
                })
                .ToList();


            // 5. Zwracamy gotowy, czytelny raport finansowy do frontendu
            return Ok(new
            {
                EmployeeName = fullName,
                Period = $"{month:D2}/{year}",
                TotalHours = totalHours,
                TotalEarnings = totalEarnings, // Teraz frontend dostanie poprawną kwotę (np. 8h * 45zł = 360zł)
                TasksDetail = taskBreakdown
            });
        }
    }
}