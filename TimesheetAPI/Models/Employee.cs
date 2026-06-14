namespace TimesheetAPI.Models;

public class Employee
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    // Relacja: jeden pracownik może mieć wiele wpisów czasu pracy
    public List<TimesheetEntry> TimesheetEntries { get; set; } = new();
}