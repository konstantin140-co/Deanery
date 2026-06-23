using System.ComponentModel.DataAnnotations;

namespace Deanery.Data.Models;

public class Student
{
    public int Id { get; set; }

    [Required, MaxLength(80)]
    public string LastName { get; set; } = string.Empty;

    [Required, MaxLength(80)]
    public string FirstName { get; set; } = string.Empty;

    [MaxLength(80)]
    public string MiddleName { get; set; } = string.Empty;

    [Required, MaxLength(20)]
    public string GroupName { get; set; } = string.Empty;

    [Required, MaxLength(30)]
    public string Phone { get; set; } = string.Empty;

    [Required, MaxLength(30)]
    public string Status { get; set; } = string.Empty;

    public DateTime EnrollmentDate { get; set; }

    public ICollection<Grade> Grades { get; set; } = new List<Grade>();

    public string FullName => string.IsNullOrWhiteSpace(MiddleName)
        ? $"{LastName} {FirstName}"
        : $"{LastName} {FirstName} {MiddleName}";
}
