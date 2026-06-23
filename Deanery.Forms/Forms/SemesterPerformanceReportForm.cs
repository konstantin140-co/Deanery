using Deanery.Data.Context;
using Deanery.Data.Helpers;
using Deanery.Forms.Ui;
using Microsoft.EntityFrameworkCore;

namespace Deanery.Forms.Forms;

public class SemesterPerformanceReportForm : TopNavShellForm
{
    private readonly AppDbContext _db = new();
    private readonly FilterComboBar _filterBar = new();
    private readonly DataGridView _grid = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = false };
    private readonly Label _lblTotals = new() { AutoSize = true, Font = UiTheme.SmallFont, Tag = "secondary" };

    public SemesterPerformanceReportForm()
    {
        SetActiveNavKey("semester-report");
        NavigationService.NavigateTo(this, "semester-report");
        UiTheme.StyleDataGrid(_grid);

        var header = new PageHeaderControl();
        header.SetTitle("Отчёт за семестр", "Успеваемость по группам");
        header.ConfigureAction("", false);
        header.HideSearch();

        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "GroupName", HeaderText = "Группа", Width = 120 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "StudentCount", HeaderText = "Студентов", Width = 100 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "GradeCount", HeaderText = "Оценок", Width = 90 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Average", HeaderText = "Средний балл", Width = 120 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Level", HeaderText = "Уровень", Width = 160 });

        var card = new RoundedPanel { Dock = DockStyle.Fill, Padding = new Padding(8) };
        card.Controls.Add(_grid);

        var footer = new Panel { Dock = DockStyle.Bottom, Height = 32, Padding = new Padding(0, 4, 0, 0) };
        footer.Controls.Add(_lblTotals);

        ContentPanel.Controls.Add(card);
        ContentPanel.Controls.Add(footer);
        ContentPanel.Controls.Add(_filterBar);
        ContentPanel.Controls.Add(header);

        _filterBar.FilterChanged += (_, _) => LoadReport();
        DataGridViewSortHelper.Attach(_grid);

        Load += (_, _) =>
        {
            LoadFilters();
            LoadReport();
        };
        FormClosed += (_, _) => _db.Dispose();
        ApplyTheme();
    }

    private void LoadFilters()
    {
        var semesters = new List<string> { "Все семестры" };
        semesters.AddRange(_db.Disciplines.Select(d => d.Semester).Distinct().OrderBy(s => s).Select(s => s.ToString()));
        _filterBar.SetSemesters(semesters, "Все семестры");
        _filterBar.SetGroups(["—"], "—");
    }

    private void LoadReport()
    {
        var selectedSemester = _filterBar.SelectedSemester;
        IQueryable<Data.Models.Grade> gradeQuery = _db.Grades
            .Include(g => g.Student)
            .Include(g => g.Discipline);

        if (!string.IsNullOrEmpty(selectedSemester) && selectedSemester != "Все семестры" &&
            int.TryParse(selectedSemester, out var semester))
        {
            gradeQuery = gradeQuery.Where(g => g.Discipline.Semester == semester);
        }

        var grades = gradeQuery.ToList();
        var groups = _db.Students.Select(s => s.GroupName).Distinct().OrderBy(g => g).ToList();

        var rows = groups.Select(group =>
        {
            var groupGrades = grades.Where(g => g.Student.GroupName == group).ToList();
            var studentIds = groupGrades.Select(g => g.StudentId).Distinct().ToList();
            var scores = groupGrades.Select(g => g.Score).ToList();
            var avg = GradeCalculator.CalculateAverage(scores);

            return new
            {
                GroupName = group,
                StudentCount = _db.Students.Count(s => s.GroupName == group),
                GradeCount = groupGrades.Count,
                Average = scores.Count == 0 ? "—" : avg.ToString("0.00"),
                Level = GradeCalculator.GetPerformanceLevel(avg)
            };
        }).Where(r => r.GradeCount > 0 || r.StudentCount > 0).ToList();

        _grid.DataSource = rows;

        var allScores = grades.Select(g => g.Score).ToList();
        var totalAvg = GradeCalculator.CalculateAverage(allScores);
        _lblTotals.Text = $"Итого групп: {rows.Count}  •  Оценок: {allScores.Count}  •  Средний балл: {(allScores.Count == 0 ? "—" : totalAvg.ToString("0.00"))}  •  {GradeCalculator.GetPerformanceLevel(totalAvg)}";
    }
}
