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

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TimesheetEntry>>> GetTimesheetEntries()
        {
            return await _context.TimesheetEntries
                .Include(t => t.Employee)
                .Include(t => t.ProjectTask)
                .ToListAsync();
        }

        
        [HttpPost]
        public async Task<ActionResult<TimesheetEntry>> PostTimesheetEntry(TimesheetEntry entry)
        {
            entry.Status = "Pending";

            
            if (entry.Date == default)
            {
                entry.Date = DateTime.Today;
            }

            _context.TimesheetEntries.Add(entry);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTimesheetEntries), new { id = entry.Id }, entry);
        }

        
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

        //Do resetu po testach
        [HttpDelete("clear-dev")]
        public async Task<IActionResult> ClearDevData()
        {
            _context.TimesheetEntries.RemoveRange(_context.TimesheetEntries);

            _context.EmployeeProjectTasks.RemoveRange(_context.EmployeeProjectTasks);

            _context.ProjectTasks.RemoveRange(_context.ProjectTasks);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Wszystkie testowe wpisy oraz zadania zostały pomyślnie usunięte z bazy!" });
        }
    }
}