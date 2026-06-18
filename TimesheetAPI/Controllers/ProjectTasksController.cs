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

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectTask>>> GetProjectTasks()
        {
            return await _context.ProjectTasks.ToListAsync();
        }

        
        [HttpPost]
        public async Task<ActionResult<ProjectTask>> PostProjectTask(ProjectTask projectTask)
        {
            _context.ProjectTasks.Add(projectTask);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProjectTasks), new { id = projectTask.Id }, projectTask);
        }


        [HttpGet("employee/{employeeId}")]
        public async Task<ActionResult<IEnumerable<ProjectTask>>> GetTasksForEmployee(int employeeId)
        {
            return await _context.EmployeeProjectTasks
                .Where(et => et.EmployeeId == employeeId)
                .Select(et => et.ProjectTask)
                .ToListAsync();
        }

        
        [HttpPost("assign")]
        public async Task<IActionResult> AssignTask([FromBody] AssignTaskDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.TaskName))
            {
                return BadRequest("Nieprawidłowe dane przydziału zadania.");
            }

            
            var projectTask = new ProjectTask
            {
                Name = dto.TaskName,
                HourlyRate = 0
            };

            _context.ProjectTasks.Add(projectTask);
            await _context.SaveChangesAsync();

            
            var employeeProjectTask = new EmployeeProjectTask
            {
                EmployeeId = dto.EmployeeId,
                ProjectTaskId = projectTask.Id
            };

            _context.EmployeeProjectTasks.Add(employeeProjectTask);
            await _context.SaveChangesAsync();

            return Ok(projectTask);
        }
    }

    
    public class AssignTaskDto
    {
        public int EmployeeId { get; set; }
        public string TaskName { get; set; } = string.Empty;
    }
}