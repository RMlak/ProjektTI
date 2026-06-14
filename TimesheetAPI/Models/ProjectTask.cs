using System.ComponentModel.DataAnnotations;

namespace TimesheetAPI.Models
{
    public class ProjectTask
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public decimal HourlyRate { get; set; }
    }
}