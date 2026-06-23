using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Deanery.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Disciplines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Teacher = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Semester = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalHours = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Disciplines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    MiddleName = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    GroupName = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    EnrollmentDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Grades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StudentId = table.Column<int>(type: "INTEGER", nullable: false),
                    DisciplineId = table.Column<int>(type: "INTEGER", nullable: false),
                    WorkType = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Score = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Grades_Disciplines_DisciplineId",
                        column: x => x.DisciplineId,
                        principalTable: "Disciplines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Grades_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Disciplines",
                columns: new[] { "Id", "Name", "Semester", "Teacher", "TotalHours" },
                values: new object[,]
                {
                    { 1, "Программирование", 3, "Смирнов А.В.", 120 },
                    { 2, "Базы данных", 3, "Кузнецова Е.Н.", 90 },
                    { 3, "Математика", 1, "Попов И.Л.", 144 },
                    { 4, "Сети и телекоммуникации", 4, "Волков Д.С.", 72 },
                    { 5, "Информационная безопасность", 5, "Лебедева О.М.", 54 }
                });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "EnrollmentDate", "FirstName", "GroupName", "LastName", "MiddleName", "Phone", "Status" },
                values: new object[,]
                {
                    { 1, new DateTime(2023, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Пётр", "ИС-21", "Иванов", "Сергеевич", "+7 (900) 111-11-11", "Обучается" },
                    { 2, new DateTime(2023, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Анна", "ИС-21", "Петрова", "Игоревна", "+7 (900) 222-22-22", "Обучается" },
                    { 3, new DateTime(2024, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Олег", "ИС-22", "Сидоров", "Андреевич", "+7 (900) 333-33-33", "Обучается" },
                    { 4, new DateTime(2024, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Мария", "ИС-22", "Козлова", "Павловна", "+7 (900) 444-44-44", "Обучается" },
                    { 5, new DateTime(2023, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Дмитрий", "ИС-21", "Новиков", "Викторович", "+7 (900) 555-55-55", "Академ" }
                });

            migrationBuilder.InsertData(
                table: "Grades",
                columns: new[] { "Id", "Date", "DisciplineId", "Score", "StudentId", "WorkType" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 5, 1, "Экзамен" },
                    { 2, new DateTime(2025, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 4, 1, "Контрольная" },
                    { 3, new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 4, 2, "Экзамен" },
                    { 4, new DateTime(2024, 12, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 2, 2, "Экзамен" },
                    { 5, new DateTime(2025, 3, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 3, 3, "Лабораторная" },
                    { 6, new DateTime(2025, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 5, 4, "Курсовая" },
                    { 7, new DateTime(2025, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 5, "Контрольная" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Grades_DisciplineId",
                table: "Grades",
                column: "DisciplineId");

            migrationBuilder.CreateIndex(
                name: "IX_Grades_StudentId",
                table: "Grades",
                column: "StudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Grades");

            migrationBuilder.DropTable(
                name: "Disciplines");

            migrationBuilder.DropTable(
                name: "Students");
        }
    }
}
