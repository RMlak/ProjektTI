using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimesheetAPI.Data;
using TimesheetAPI.Models;

namespace TimesheetAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectTasksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProjectTasksController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/ProjectTasks
        // Pobiera listę wszystkich zadań (dostępne dla każdego)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectTask>>> GetProjectTasks()
        {
            return await _context.ProjectTasks.ToListAsync();
        }

        // POST: api/ProjectTasks
        // Dodaje nowe zadanie ze stawką godzinową
        [HttpPost]
        public async Task<ActionResult<ProjectTask>> PostProjectTask(ProjectTask projectTask)
        {
            _context.ProjectTasks.Add(projectTask);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProjectTasks), new { id = projectTask.Id }, projectTask);
        }

        // === NOWOŚĆ ===
        // GET: api/ProjectTasks/employee/5
        // Pobiera zadania przypisane TYLKO do zalogowanego pracownika
        [HttpGet("employee/{employeeId}")]
        public async Task<ActionResult<IEnumerable<ProjectTask>>> GetTasksForEmployee(int employeeId)
        {
            return await _context.EmployeeProjectTasks
                .Where(et => et.EmployeeId == employeeId)
                .Select(et => et.ProjectTask)
                .ToListAsync();
        }

        // === NOWOŚĆ ===
        // POST: api/ProjectTasks/assign
        // Tworzy nowe zadanie i od razu przypisuje je wybranemu pracownikowi
        [HttpPost("assign")]
        public async Task<IActionResult> AssignTask([FromBody] AssignTaskDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.TaskName))
            {
                return BadRequest("Nieprawidłowe dane przydziału zadania.");
            }

            // 1. Tworzymy nowe zadanie projektowe. 
            // Ponieważ pracownicy mają odgórną stawkę, HourlyRate ustawiamy na 0.
            var projectTask = new ProjectTask
            {
                Name = dto.TaskName,
                HourlyRate = 0
            };

            _context.ProjectTasks.Add(projectTask);
            await _context.SaveChangesAsync(); // Zapisujemy, aby baza wygenerowała dla zadania nowe Id

            // 2. Łączymy nowo utworzone zadanie z pracownikiem w tabeli pośredniej
            var employeeProjectTask = new EmployeeProjectTask
            {
                EmployeeId = dto.EmployeeId,
                ProjectTaskId = projectTask.Id
            };

            _context.EmployeeProjectTasks.Add(employeeProjectTask);
            await _context.SaveChangesAsync(); // Zapisujemy powiązanie

            return Ok(projectTask);
        }
    }

    // Klasa pomocnicza (DTO) do odbierania danych z formularza Admina na frontendzie
    public class AssignTaskDto
    {
        public int EmployeeId { get; set; }
        public string TaskName { get; set; } = string.Empty;
    }
}