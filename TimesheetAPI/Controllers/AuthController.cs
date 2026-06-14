using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimesheetAPI.Data;

namespace TimesheetAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        // Pomocnicza klasa, która odbiera login i hasło ze strony internetowej
        public class LoginDto
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        // POST: api/Auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginData)
        {
            // Szukamy w bazie pracownika, który ma dokładnie taki login I takie hasło
            var user = await _context.Employees
                .FirstOrDefaultAsync(e => e.Username == loginData.Username && e.Password == loginData.Password);

            // Jeśli nie znaleźliśmy nikogo – odrzucamy dostęp
            if (user == null)
            {
                return Unauthorized(new { message = "Błędny login lub hasło!" });
            }

            
            return Ok(new
            {
                EmployeeId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role 
            });
        }
    }
}