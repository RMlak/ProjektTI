using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimesheetAPI.Data;
using TimesheetAPI.Models;

namespace TimesheetAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimesheetEntriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TimesheetEntriesController(AppDbContext context)
        {
            _context = context;
        }

        // 1. POBIERANIE: GET: api/TimesheetEntries
        // Zwraca wszystkie wpisy (razem z pełnymi danymi pracownika i zadania - super pod raporty Admina!)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TimesheetEntry>>> GetTimesheetEntries()
        {
            return await _context.TimesheetEntries
                .Include(t => t.Employee)
                .Include(t => t.ProjectTask)
                .ToListAsync();
        }

        // 2. DODAWANIE: POST: api/TimesheetEntries
        // Użytkownik wysyła swój dzienny raport. Status automatycznie ustawia się jako "Pending".
        [HttpPost]
        public async Task<ActionResult<TimesheetEntry>> PostTimesheetEntry(TimesheetEntry entry)
        {
            // Na wypadek gdyby ktoś próbował oszukać system - nowy wpis zawsze startuje jako oczekujący
            entry.Status = "Pending";
            entry.Date = DateTime.Now; // Automatycznie przypisujemy dzisiejszą datę

            _context.TimesheetEntries.Add(entry);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTimesheetEntries), new { id = entry.Id }, entry);
        }

        // 3. AKCEPTACJA/ODRZUCENIE: PUT: api/TimesheetEntries/{id}/status
        // Punkt dla Admina/Menadżera do akceptowania ("Approved") lub odrzucania ("Rejected") godzin
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string newStatus)
        {
            if (newStatus != "Approved" && newStatus != "Rejected")
            {
                return BadRequest("Niedozwolony status. Wybierz 'Approved' lub 'Rejected'.");
            }

            var entry = await _context.TimesheetEntries.FindAsync(id);
            if (entry == null)
            {
                return NotFound("Nie znaleziono takiego wpisu czasu pracy.");
            }

            entry.Status = newStatus;
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Status wpisu zaktualizowany na: {newStatus}" });
        }
    }
}