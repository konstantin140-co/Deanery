using Deanery.Data.Context;
using Deanery.Forms.Ui;
using Microsoft.EntityFrameworkCore;

namespace Deanery.Forms.Forms;

public class DisciplineSummaryReportForm : TopNavShellForm
{
    private readonly AppDbContext _db = new();
    private readonly DateTimePicker _dtpFrom = new() { Width = 140, Format = DateTimePickerFormat.Short };
    private readonly DateTimePicker _dtpTo = new() { Width = 140, Format = DateTimePickerFormat.Short };
    private readonly DataGridView _grid = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = false };
    private readonly Label _lblTotals = new() { AutoSize = true, Font = UiTheme.SmallFont, Tag = "secondary" };

    public DisciplineSummaryReportForm()
    {
        SetActiveNavKey("discipline-report");
        NavigationService.NavigateTo(this, "discipline-report");
        UiTheme.StyleDataGrid(_grid);

        _dtpFrom.Value = DateTime.Today.AddMonths(-6);
        _dtpTo.Value = DateTime.Today;

        var header = new PageHeaderControl();
        header.SetTitle("Отчёт по дисциплине", "Распределение оценок за период");
        header.ConfigureAction("", false);
        header.HideSearch();

        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DisciplineName", HeaderText = "Дисциплина", Width = 200 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Count2", HeaderText = "«2»", Width = 60 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Count3", HeaderText = "«3»", Width = 60 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Count4", HeaderText = "«4»", Width = 60 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Count5", HeaderText = "«5»", Width = 60 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Total", HeaderText = "Всего", Width = 70 });

        var filter = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 44, Padding = new Padding(0, 4, 0, 8), BackColor = Color.Transparent };
        filter.Controls.Add(new Label { Text = "С:", AutoSize = true, Padding = new Padding(0, 8, 0, 0), Tag = "secondary" });
        filter.Controls.Add(_dtpFrom);
        filter.Controls.Add(new Label { Text = "По:", AutoSize = true, Padding = new Padding(8, 8, 0, 0), Tag = "secondary" });
        filter.Controls.Add(_dtpTo);
        var btnApply = new Button { Text = "Применить", Width = 110 };
        UiTheme.StylePrimaryButton(btnApply);
        btnApply.Click += (_, _) => LoadReport();
        filter.Controls.Add(btnApply);

        var card = new RoundedPanel { Dock = DockStyle.Fill, Padding = new Padding(8) };
        card.Controls.Add(_grid);

        var footer = new Panel { Dock = DockStyle.Bottom, Height = 32, Padding = new Padding(0, 4, 0, 0) };
        footer.Controls.Add(_lblTotals);

        ContentPanel.Controls.Add(card);
        ContentPanel.Controls.Add(footer);
        ContentPanel.Controls.Add(filter);
        ContentPanel.Controls.Add(header);

        DataGridViewSortHelper.Attach(_grid);
        Load += (_, _) => LoadReport();
        FormClosed += (_, _) => _db.Dispose();
        ApplyTheme();
    }

    private void LoadReport()
    {
        var from = _dtpFrom.Value.Date;
        var to = _dtpTo.Value.Date.AddDays(1);

        var grades = _db.Grades
            .Include(g => g.Discipline)
            .Where(g => g.Date >= from && g.Date < to)
            .ToList();

        var rows = grades
            .GroupBy(g => g.Discipline.Name)
            .Select(g => new
            {
                DisciplineName = g.Key,
                Count2 = g.Count(x => x.Score == 2),
                Count3 = g.Count(x => x.Score == 3),
                Count4 = g.Count(x => x.Score == 4),
                Count5 = g.Count(x => x.Score == 5),
                Total = g.Count()
            })
            .OrderBy(r => r.DisciplineName)
            .ToList();

        _grid.DataSource = rows;
        _lblTotals.Text = $"Дисциплин: {rows.Count}  •  Оценок «2»: {rows.Sum(r => r.Count2)}  •  «3»: {rows.Sum(r => r.Count3)}  •  «4»: {rows.Sum(r => r.Count4)}  •  «5»: {rows.Sum(r => r.Count5)}  •  Всего: {rows.Sum(r => r.Total)}";
    }
}
