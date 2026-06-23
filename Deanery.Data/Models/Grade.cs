using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Deanery.Data.Models;

public class Grade
{
    public int Id { get; set; }

    public int StudentId { get; set; }
    public Student Student { get; set; } = null!;

    public int DisciplineId { get; set; }
    public Discipline Discipline { get; set; } = null!;

    [Required, MaxLength(40)]
    public string WorkType { get; set; } = string.Empty;

    public DateTime Date { get; set; }

    [Range(2, 5)]
    public int Score { get; set; }
}
