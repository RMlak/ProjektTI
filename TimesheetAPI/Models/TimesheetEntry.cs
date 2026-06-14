using System;
using System.ComponentModel.DataAnnotations;

namespace TimesheetAPI.Models
{
    public class TimesheetEntry
    {
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; } // Kto pracował
        public Employee? Employee { get; set; }

        [Required]
        public int ProjectTaskId { get; set; } // Nad czym pracował
        public ProjectTask? ProjectTask { get; set; }

        [Required]
        public double Hours { get; set; } // Ile godzin

        [Required]
        public string Description { get; set; } = string.Empty; // Opis wykonanej pracy

        [Required]
        public DateTime Date { get; set; } // Kiedy

        [Required]
        public string Status { get; set; } = "Pending"; // Status: Pending, Approved, Rejected
    }
}