using Deanery.Data.Context;
using Deanery.Data.Helpers;
using Deanery.Forms.Ui;
using Microsoft.EntityFrameworkCore;

namespace Deanery.Forms.Forms;

public class MainForm : TopNavShellForm
{
    private readonly AppDbContext _db = new();
    private readonly Label _lblStudents = CreateStatValue();
    private readonly Label _lblDisciplines = CreateStatValue();
    private readonly Label _lblDebtors = CreateStatValue();
    private readonly Label _lblAvgGrade = CreateStatValue();

    public MainForm()
    {
        SetActiveNavKey("home");
        NavigationService.NavigateTo(this, "home");
        BuildDashboard();
        Load += (_, _) => LoadStats();
        FormClosed += (_, _) => _db.Dispose();
        ApplyTheme();
    }

    private void BuildDashboard()
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            BackColor = Color.Transparent,
            Tag = "theme-bg"
        };
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var welcome = new Label
        {
            Text = "Деканат",
            Font = UiTheme.TitleFont,
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 8),
            Tag = "primary"
        };

        var subtitle = new Label
        {
            Text = "Сводка по студентам, дисциплинам и успеваемости",
            Font = UiTheme.BodyFont,
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 24),
            Tag = "secondary"
        };

        var cards = new FlowLayoutPanel
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            WrapContents = true,
            BackColor = Color.Transparent,
            Margin = new Padding(0)
        };

        cards.Controls.Add(CreateStatTile("Студенты", "Всего в базе", _lblStudents, UiTheme.Accent));
        cards.Controls.Add(CreateStatTile("Дисциплины", "Учебные предметы", _lblDisciplines, Color.FromArgb(59, 130, 246)));
        cards.Controls.Add(CreateStatTile("Должники", "Оценка «2»", _lblDebtors, Color.FromArgb(239, 68, 68)));
        cards.Controls.Add(CreateStatTile("Средний балл", "По колледжу", _lblAvgGrade, Color.FromArgb(34, 197, 94)));

        layout.Controls.Add(welcome, 0, 0);
        layout.Controls.Add(subtitle, 0, 1);
        layout.Controls.Add(cards, 0, 2);

        ContentPanel.Controls.Add(layout);
    }

    private static Label CreateStatValue() => new()
    {
        AutoSize = true,
        Font = new Font("Segoe UI", 28F, FontStyle.Bold),
        Tag = "accent"
    };

    private static RoundedPanel CreateStatTile(string title, string desc, Label valueLabel, Color accent)
    {
        var card = new RoundedPanel { Size = new Size(260, 140), Margin = new Padding(0, 0, 16, 16) };
        var strip = new Panel
        {
            Size = new Size(4, 140),
            Location = new Point(0, 0),
            BackColor = accent
        };
        var titleLbl = new Label
        {
            Text = title,
            Location = new Point(20, 16),
            AutoSize = true,
            Font = new Font("Segoe UI", 12F, FontStyle.Bold),
            Tag = "primary"
        };
        var descLbl = new Label
        {
            Text = desc,
            Location = new Point(20, 42),
            AutoSize = true,
            Font = UiTheme.SmallFont,
            Tag = "secondary"
        };
        valueLabel.Location = new Point(20, 72);
        card.Controls.AddRange([strip, titleLbl, descLbl, valueLabel]);
        return card;
    }

    private void LoadStats()
    {
        var studentCount = _db.Students.Count();
        var disciplineCount = _db.Disciplines.Count();
        var debtorIds = _db.Grades.Where(g => g.Score == 2).Select(g => g.StudentId).Distinct().Count();
        var scores = _db.Grades.Select(g => g.Score).ToList();
        var avg = GradeCalculator.CalculateAverage(scores);

        _lblStudents.Text = studentCount.ToString();
        _lblDisciplines.Text = disciplineCount.ToString();
        _lblDebtors.Text = debtorIds.ToString();
        _lblAvgGrade.Text = scores.Count == 0 ? "—" : avg.ToString("0.00");
    }
}
