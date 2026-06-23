using Deanery.Data.Context;
using Deanery.Forms.Ui;
using Microsoft.EntityFrameworkCore;

namespace Deanery.Forms.Forms;

public class DisciplineListForm : TopNavShellForm
{
    private readonly AppDbContext _db = new();
    private readonly PageHeaderControl _header = new();
    private readonly FilterComboBar _filterBar = new();
    private readonly ListRowPanel _listPanel = new();
    private readonly Label _lblCount = new() { AutoSize = true, Font = UiTheme.SmallFont, Tag = "secondary" };
    private readonly System.Windows.Forms.Timer _searchTimer = new() { Interval = 300 };

    public DisciplineListForm()
    {
        SetActiveNavKey("disciplines");
        NavigationService.NavigateTo(this, "disciplines");

        _header.SetTitle("Дисциплины", "Учебные дисциплины");
        _header.ConfigureAction("Добавить");

        var footer = new Panel { Dock = DockStyle.Bottom, Height = 28, Padding = new Padding(0, 4, 0, 0) };
        footer.Controls.Add(_lblCount);

        ContentPanel.Controls.Add(_listPanel);
        ContentPanel.Controls.Add(footer);
        ContentPanel.Controls.Add(_filterBar);
        ContentPanel.Controls.Add(_header);

        _header.ActionClicked += (_, _) => EditRecord(0);
        _header.SearchTextChanged += (_, _) => { _searchTimer.Stop(); _searchTimer.Start(); };
        _searchTimer.Tick += (_, _) => { _searchTimer.Stop(); LoadData(_header.SearchBox.Text); };
        _filterBar.FilterChanged += (_, _) => LoadData(_header.SearchBox.Text);
        _listPanel.RowEditClicked += (_, id) => EditRecord(id);
        _listPanel.RowDeleteClicked += (_, id) => DeleteRecord(id);

        Load += (_, _) =>
        {
            LoadFilters();
            LoadData();
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

    private void LoadData(string filter = "")
    {
        var query = _db.Disciplines.AsQueryable();

        var selectedSemester = _filterBar.SelectedSemester;
        if (!string.IsNullOrEmpty(selectedSemester) && selectedSemester != "Все семестры" && int.TryParse(selectedSemester, out var semester))
            query = query.Where(d => d.Semester == semester);

        if (!string.IsNullOrWhiteSpace(filter))
        {
            var f = filter.Trim();
            query = query.Where(d => d.Name.Contains(f) || d.Teacher.Contains(f));
        }

        var items = query.OrderBy(d => d.Semester).ThenBy(d => d.Name).ToList();
        _listPanel.ClearRows();

        foreach (var item in items)
        {
            _listPanel.AddRow(
                item.Id,
                item.Name,
                $"{item.Semester} сем.",
                $"Преподаватель: {item.Teacher}  •  Часов: {item.TotalHours}");
        }

        _lblCount.Text = $"Записей: {items.Count}";
    }

    private void EditRecord(int id)
    {
        using var form = new DisciplineEditForm(id);
        if (form.ShowDialog() == DialogResult.OK)
            LoadData(_header.SearchBox.Text);
    }

    private void DeleteRecord(int id)
    {
        try
        {
            var item = _db.Disciplines.Find(id);
            if (item == null) return;

            if (MessageBox.Show($"Удалить «{item.Name}»? Это действие нельзя отменить.", "Подтверждение",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            _db.Disciplines.Remove(item);
            _db.SaveChanges();
            LoadFilters();
            LoadData(_header.SearchBox.Text);
        }
        catch (DbUpdateException)
        {
            MessageBox.Show("Нельзя удалить запись: есть связанные оценки.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
