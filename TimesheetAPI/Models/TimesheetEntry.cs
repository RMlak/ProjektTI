using System;
using System.ComponentModel.DataAnnotations;

namespace TimesheetAPI.Models
{
    public class TimesheetEntry
    {
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        [Required]
        public int ProjectTaskId { get; set; }
        public ProjectTask? ProjectTask { get; set; }

        [Required]
        public double Hours { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string Status { get; set; } = "Pending";
    }
}