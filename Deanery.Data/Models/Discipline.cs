using System.ComponentModel.DataAnnotations;

namespace Deanery.Data.Models;

public class Discipline
{
    public int Id { get; set; }

    [Required, MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Teacher { get; set; } = string.Empty;

    [Range(1, 12)]
    public int Semester { get; set; }

    [Range(1, 500)]
    public int TotalHours { get; set; }

    public ICollection<Grade> Grades { get; set; } = new List<Grade>();
}
