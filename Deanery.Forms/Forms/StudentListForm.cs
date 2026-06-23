using Deanery.Data.Context;
using Deanery.Data.Models;
using Deanery.Forms.Ui;
using Microsoft.EntityFrameworkCore;

namespace Deanery.Forms.Forms;

public class StudentListForm : TopNavShellForm
{
    private readonly AppDbContext _db = new();
    private readonly PageHeaderControl _header = new();
    private readonly FilterComboBar _filterBar = new();
    private readonly ListRowPanel _listPanel = new();
    private readonly Label _lblCount = new() { AutoSize = true, Font = UiTheme.SmallFont, Tag = "secondary" };
    private readonly System.Windows.Forms.Timer _searchTimer = new() { Interval = 300 };

    public StudentListForm()
    {
        SetActiveNavKey("students");
        NavigationService.NavigateTo(this, "students");

        _header.SetTitle("Студенты", "Список обучающихся");
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
        var groups = new List<string> { "Все группы" };
        groups.AddRange(_db.Students.Select(s => s.GroupName).Distinct().OrderBy(g => g));
        _filterBar.SetGroups(groups, "Все группы");
        _filterBar.SetSemesters(["—"], "—");
    }

    private void LoadData(string filter = "")
    {
        var query = _db.Students.AsQueryable();

        var selectedGroup = _filterBar.SelectedGroup;
        if (!string.IsNullOrEmpty(selectedGroup) && selectedGroup != "Все группы")
            query = query.Where(s => s.GroupName == selectedGroup);

        if (!string.IsNullOrWhiteSpace(filter))
        {
            var f = filter.Trim();
            query = query.Where(s =>
                s.LastName.Contains(f) ||
                s.FirstName.Contains(f) ||
                s.MiddleName.Contains(f) ||
                s.GroupName.Contains(f) ||
                s.Phone.Contains(f));
        }

        var items = query.OrderBy(s => s.LastName).ThenBy(s => s.FirstName).ToList();
        _listPanel.ClearRows();

        foreach (var item in items)
        {
            _listPanel.AddRow(
                item.Id,
                item.FullName,
                item.Status,
                $"Группа: {item.GroupName}  •  Тел.: {item.Phone}  •  Зачислен: {item.EnrollmentDate:dd.MM.yyyy}");
        }

        _lblCount.Text = $"Записей: {items.Count}";
    }

    private void EditRecord(int id)
    {
        using var form = new StudentEditForm(id);
        if (form.ShowDialog() == DialogResult.OK)
            LoadData(_header.SearchBox.Text);
    }

    private void DeleteRecord(int id)
    {
        try
        {
            var item = _db.Students.Find(id);
            if (item == null) return;

            if (MessageBox.Show($"Удалить «{item.FullName}»? Это действие нельзя отменить.", "Подтверждение",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            _db.Students.Remove(item);
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
