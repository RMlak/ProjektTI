using TimesheetAPI.Models;

public class EmployeeProjectTask
{
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; }

    public int ProjectTaskId { get; set; }
    public ProjectTask ProjectTask { get; set; }
}