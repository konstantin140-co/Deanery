using Deanery.Data.Context;
using Deanery.Data.Helpers;
using Deanery.Forms.Ui;
using Microsoft.EntityFrameworkCore;

namespace Deanery.Forms.Forms;

public class GroupGradesForm : TopNavShellForm
{
    private readonly AppDbContext _db = new();
    private readonly ComboBox _cmbGroup = new() { Width = 160, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _cmbDiscipline = new() { Width = 260, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly DataGridView _grid = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = false };
    private readonly Label _lblSummary = new() { AutoSize = true, Font = UiTheme.SmallFont, Tag = "secondary" };

    public GroupGradesForm()
    {
        SetActiveNavKey("group-grades");
        NavigationService.NavigateTo(this, "group-grades");
        UiTheme.StyleDataGrid(_grid);

        var header = new PageHeaderControl();
        header.SetTitle("Оценки группы", "Успеваемость по дисциплине");
        header.ConfigureAction("", false);
        header.HideSearch();

        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "StudentName", HeaderText = "Студент", Width = 220 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "GroupName", HeaderText = "Группа", Width = 90 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Scores", HeaderText = "Оценки", Width = 180 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Average", HeaderText = "Средний балл", Width = 110 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Level", HeaderText = "Уровень", Width = 140 });

        var filter = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 44, Padding = new Padding(0, 4, 0, 8), BackColor = Color.Transparent };
        filter.Controls.Add(new Label { Text = "Группа:", AutoSize = true, Padding = new Padding(0, 8, 8, 0), Tag = "secondary" });
        filter.Controls.Add(_cmbGroup);
        filter.Controls.Add(new Label { Text = "Дисциплина:", AutoSize = true, Padding = new Padding(8, 8, 8, 0), Tag = "secondary" });
        filter.Controls.Add(_cmbDiscipline);

        _cmbGroup.SelectedIndexChanged += (_, _) => LoadGrid();
        _cmbDiscipline.SelectedIndexChanged += (_, _) => LoadGrid();

        var card = new RoundedPanel { Dock = DockStyle.Fill, Padding = new Padding(8) };
        card.Controls.Add(_grid);

        var footer = new Panel { Dock = DockStyle.Bottom, Height = 32, Padding = new Padding(0, 4, 0, 0) };
        footer.Controls.Add(_lblSummary);

        ContentPanel.Controls.Add(card);
        ContentPanel.Controls.Add(footer);
        ContentPanel.Controls.Add(filter);
        ContentPanel.Controls.Add(header);

        DataGridViewSortHelper.Attach(_grid);

        Load += (_, _) =>
        {
            LoadFilters();
            LoadGrid();
        };
        FormClosed += (_, _) => _db.Dispose();
        ApplyTheme();
    }

    private void LoadFilters()
    {
        var groups = _db.Students.Select(s => s.GroupName).Distinct().OrderBy(g => g).ToList();
        _cmbGroup.Items.Clear();
        foreach (var group in groups)
            _cmbGroup.Items.Add(group);
        if (_cmbGroup.Items.Count > 0)
            _cmbGroup.SelectedIndex = 0;

        var disciplines = _db.Disciplines.OrderBy(d => d.Name).ToList();
        _cmbDiscipline.DisplayMember = "Name";
        _cmbDiscipline.ValueMember = "Id";
        _cmbDiscipline.DataSource = disciplines;
    }

    private void LoadGrid()
    {
        if (_cmbGroup.SelectedItem is not string selectedGroup ||
            _cmbDiscipline.SelectedItem is not Data.Models.Discipline discipline)
        {
            _grid.DataSource = null;
            _lblSummary.Text = "Выберите группу и дисциплину";
            return;
        }

        var students = _db.Students
            .Where(s => s.GroupName == selectedGroup)
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName)
            .ToList();

        var grades = _db.Grades
            .Where(g => g.DisciplineId == discipline.Id)
            .ToLookup(g => g.StudentId);

        var rows = students.Select(s =>
        {
            var scores = grades[s.Id].Select(g => g.Score).ToList();
            var avg = GradeCalculator.CalculateAverage(scores);
            return new
            {
                StudentName = s.FullName,
                GroupName = s.GroupName,
                Scores = scores.Count == 0 ? "—" : string.Join(", ", scores),
                Average = scores.Count == 0 ? "—" : avg.ToString("0.00"),
                Level = GradeCalculator.GetPerformanceLevel(avg)
            };
        }).ToList();

        _grid.DataSource = rows;
        _lblSummary.Text = $"Группа: {selectedGroup}  •  Дисциплина: {discipline.Name}  •  Студентов: {rows.Count}";
    }
}
