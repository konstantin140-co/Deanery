using Deanery.Data.Context;
using Deanery.Forms.Ui;
using Microsoft.EntityFrameworkCore;

namespace Deanery.Forms.Forms;

public class GradeListForm : TopNavShellForm
{
    private readonly AppDbContext _db = new();
    private readonly PageHeaderControl _header = new();
    private readonly ListRowPanel _listPanel = new();
    private readonly Label _lblCount = new() { AutoSize = true, Font = UiTheme.SmallFont, Tag = "secondary" };
    private readonly System.Windows.Forms.Timer _searchTimer = new() { Interval = 300 };

    public GradeListForm()
    {
        SetActiveNavKey("grades");
        NavigationService.NavigateTo(this, "grades");

        _header.SetTitle("Оценки", "Журнал успеваемости");
        _header.ConfigureAction("Добавить");

        var footer = new Panel { Dock = DockStyle.Bottom, Height = 28, Padding = new Padding(0, 4, 0, 0) };
        footer.Controls.Add(_lblCount);

        ContentPanel.Controls.Add(_listPanel);
        ContentPanel.Controls.Add(footer);
        ContentPanel.Controls.Add(_header);

        _header.ActionClicked += (_, _) => EditRecord(0);
        _header.SearchTextChanged += (_, _) => { _searchTimer.Stop(); _searchTimer.Start(); };
        _searchTimer.Tick += (_, _) => { _searchTimer.Stop(); LoadData(_header.SearchBox.Text); };
        _listPanel.RowEditClicked += (_, id) => EditRecord(id);
        _listPanel.RowDeleteClicked += (_, id) => DeleteRecord(id);

        Load += (_, _) => LoadData();
        FormClosed += (_, _) => _db.Dispose();
        ApplyTheme();
    }

    private void LoadData(string filter = "")
    {
        var query = _db.Grades
            .Include(g => g.Student)
            .Include(g => g.Discipline)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter))
        {
            var f = filter.Trim();
            query = query.Where(g =>
                g.Student.LastName.Contains(f) ||
                g.Student.FirstName.Contains(f) ||
                g.Discipline.Name.Contains(f) ||
                g.WorkType.Contains(f));
        }

        var items = query.OrderByDescending(g => g.Date).ThenBy(g => g.Student.LastName).ToList();
        _listPanel.ClearRows();

        foreach (var item in items)
        {
            _listPanel.AddRowWithScore(
                item.Id,
                $"{item.Student.FullName} — {item.Discipline.Name}",
                item.Score,
                $"{item.WorkType}  •  {item.Date:dd.MM.yyyy}  •  Оценка: {item.Score}");
        }

        _lblCount.Text = $"Записей: {items.Count}";
    }

    private void EditRecord(int id)
    {
        using var form = new GradeEditForm(id);
        if (form.ShowDialog() == DialogResult.OK)
            LoadData(_header.SearchBox.Text);
    }

    private void DeleteRecord(int id)
    {
        var item = _db.Grades.Find(id);
        if (item == null) return;

        if (MessageBox.Show("Удалить эту оценку? Это действие нельзя отменить.", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        _db.Grades.Remove(item);
        _db.SaveChanges();
        LoadData(_header.SearchBox.Text);
    }
}
