namespace TimesheetAPI.Models;

public class TimesheetEntry
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int HoursWorked { get; set; }
    public string Description { get; set; } = string.Empty;

    // Połączenie z pracownikiem (Klucz obcy)
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }
}