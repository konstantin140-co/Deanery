using Deanery.Data.Constants;
using Deanery.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Deanery.Data.Context;

public class AppDbContext : DbContext
{
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Discipline> Disciplines => Set<Discipline>();
    public DbSet<Grade> Grades => Set<Grade>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=deanery.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Grade>()
            .HasOne(g => g.Student)
            .WithMany(s => s.Grades)
            .HasForeignKey(g => g.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Grade>()
            .HasOne(g => g.Discipline)
            .WithMany(d => d.Grades)
            .HasForeignKey(g => g.DisciplineId)
            .OnDelete(DeleteBehavior.Restrict);

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>().HasData(
            new Student { Id = 1, LastName = "Иванов", FirstName = "Пётр", MiddleName = "Сергеевич", GroupName = "ИС-21", Phone = "+7 (900) 111-11-11", Status = StudentStatuses.Active, EnrollmentDate = new DateTime(2023, 9, 1) },
            new Student { Id = 2, LastName = "Петрова", FirstName = "Анна", MiddleName = "Игоревна", GroupName = "ИС-21", Phone = "+7 (900) 222-22-22", Status = StudentStatuses.Active, EnrollmentDate = new DateTime(2023, 9, 1) },
            new Student { Id = 3, LastName = "Сидоров", FirstName = "Олег", MiddleName = "Андреевич", GroupName = "ИС-22", Phone = "+7 (900) 333-33-33", Status = StudentStatuses.Active, EnrollmentDate = new DateTime(2024, 9, 1) },
            new Student { Id = 4, LastName = "Козлова", FirstName = "Мария", MiddleName = "Павловна", GroupName = "ИС-22", Phone = "+7 (900) 444-44-44", Status = StudentStatuses.Active, EnrollmentDate = new DateTime(2024, 9, 1) },
            new Student { Id = 5, LastName = "Новиков", FirstName = "Дмитрий", MiddleName = "Викторович", GroupName = "ИС-21", Phone = "+7 (900) 555-55-55", Status = StudentStatuses.AcademicLeave, EnrollmentDate = new DateTime(2023, 9, 1) }
        );

        modelBuilder.Entity<Discipline>().HasData(
            new Discipline { Id = 1, Name = "Программирование", Teacher = "Смирнов А.В.", Semester = 3, TotalHours = 120 },
            new Discipline { Id = 2, Name = "Базы данных", Teacher = "Кузнецова Е.Н.", Semester = 3, TotalHours = 90 },
            new Discipline { Id = 3, Name = "Математика", Teacher = "Попов И.Л.", Semester = 1, TotalHours = 144 },
            new Discipline { Id = 4, Name = "Сети и телекоммуникации", Teacher = "Волков Д.С.", Semester = 4, TotalHours = 72 },
            new Discipline { Id = 5, Name = "Информационная безопасность", Teacher = "Лебедева О.М.", Semester = 5, TotalHours = 54 }
        );

        modelBuilder.Entity<Grade>().HasData(
            new Grade { Id = 1, StudentId = 1, DisciplineId = 1, WorkType = WorkTypes.Exam, Date = new DateTime(2025, 1, 15), Score = 5 },
            new Grade { Id = 2, StudentId = 1, DisciplineId = 2, WorkType = WorkTypes.Test, Date = new DateTime(2025, 2, 10), Score = 4 },
            new Grade { Id = 3, StudentId = 2, DisciplineId = 1, WorkType = WorkTypes.Exam, Date = new DateTime(2025, 1, 15), Score = 4 },
            new Grade { Id = 4, StudentId = 2, DisciplineId = 3, WorkType = WorkTypes.Exam, Date = new DateTime(2024, 12, 20), Score = 2 },
            new Grade { Id = 5, StudentId = 3, DisciplineId = 4, WorkType = WorkTypes.Lab, Date = new DateTime(2025, 3, 5), Score = 3 },
            new Grade { Id = 6, StudentId = 4, DisciplineId = 2, WorkType = WorkTypes.Coursework, Date = new DateTime(2025, 4, 1), Score = 5 },
            new Grade { Id = 7, StudentId = 5, DisciplineId = 1, WorkType = WorkTypes.Test, Date = new DateTime(2025, 2, 1), Score = 2 }
        );
    }
}
