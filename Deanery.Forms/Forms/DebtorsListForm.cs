using Deanery.Data.Context;
using Deanery.Data.Helpers;
using Deanery.Forms.Ui;
using Microsoft.EntityFrameworkCore;

namespace Deanery.Forms.Forms;

public class DebtorsListForm : TopNavShellForm
{
    private readonly AppDbContext _db = new();
    private readonly PageHeaderControl _header = new();
    private readonly ListRowPanel _listPanel = new();
    private readonly Label _lblCount = new() { AutoSize = true, Font = UiTheme.SmallFont, Tag = "secondary" };
    private readonly System.Windows.Forms.Timer _searchTimer = new() { Interval = 300 };

    public DebtorsListForm()
    {
        SetActiveNavKey("debtors");
        NavigationService.NavigateTo(this, "debtors");

        _header.SetTitle("Должники", "Студенты с оценкой «2»");
        _header.ConfigureAction("", false);

        var footer = new Panel { Dock = DockStyle.Bottom, Height = 28, Padding = new Padding(0, 4, 0, 0) };
        footer.Controls.Add(_lblCount);

        ContentPanel.Controls.Add(_listPanel);
        ContentPanel.Controls.Add(footer);
        ContentPanel.Controls.Add(_header);

        _header.SearchTextChanged += (_, _) => { _searchTimer.Stop(); _searchTimer.Start(); };
        _searchTimer.Tick += (_, _) => { _searchTimer.Stop(); LoadData(_header.SearchBox.Text); };

        Load += (_, _) => LoadData();
        FormClosed += (_, _) => _db.Dispose();
        ApplyTheme();
    }

    private void LoadData(string filter = "")
    {
        var debtorStudentIds = _db.Grades
            .Where(g => g.Score == 2)
            .Select(g => g.StudentId)
            .Distinct()
            .ToList();

        var query = _db.Students
            .Include(s => s.Grades)
            .ThenInclude(g => g.Discipline)
            .Where(s => debtorStudentIds.Contains(s.Id))
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter))
        {
            var f = filter.Trim();
            query = query.Where(s =>
                s.LastName.Contains(f) ||
                s.FirstName.Contains(f) ||
                s.GroupName.Contains(f));
        }

        var items = query.OrderBy(s => s.LastName).ThenBy(s => s.FirstName).ToList();
        _listPanel.ClearRows();

        foreach (var student in items)
        {
            var failingGrades = student.Grades.Where(g => g.Score == 2).ToList();
            var disciplines = string.Join(", ", failingGrades.Select(g => g.Discipline.Name).Distinct());
            var avg = GradeCalculator.CalculateAverage(student.Grades.Select(g => g.Score));

            _listPanel.AddRow(
                student.Id,
                student.FullName,
                "Должник",
                $"Группа: {student.GroupName}  •  Долги: {disciplines}  •  Средний балл: {avg:0.00}");
        }

        _lblCount.Text = $"Должников: {items.Count}";
    }
}
