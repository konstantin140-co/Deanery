using Deanery.Forms.Forms;

namespace Deanery.Forms.Ui;

public class TopNavShellForm : Form
{
    protected readonly Panel ContentPanel;
    private readonly TopNavBar _topNav = new();
    private readonly ToolStripMenuItem _lightThemeItem;
    private readonly ToolStripMenuItem _darkThemeItem;

    protected TopNavShellForm()
    {
        Text = "Деканат";
        MinimumSize = new Size(1100, 650);
        Size = new Size(1200, 720);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = UiTheme.Background;
        Font = UiTheme.BodyFont;

        var menu = new MenuStrip();
        var fileMenu = new ToolStripMenuItem("Файл");
        fileMenu.DropDownItems.Add("Выход", null, (_, _) => Application.Exit());

        var viewMenu = new ToolStripMenuItem("Вид");
        _lightThemeItem = new ToolStripMenuItem("Светлая тема") { Checked = UiTheme.Current == AppTheme.Light };
        _darkThemeItem = new ToolStripMenuItem("Тёмная тема") { Checked = UiTheme.Current == AppTheme.Dark };
        _lightThemeItem.Click += (_, _) => SetTheme(AppTheme.Light);
        _darkThemeItem.Click += (_, _) => SetTheme(AppTheme.Dark);
        viewMenu.DropDownItems.AddRange([_lightThemeItem, _darkThemeItem]);

        menu.Items.AddRange([fileMenu, viewMenu]);
        UiTheme.StyleMenuStrip(menu);
        MainMenuStrip = menu;

        ContentPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(UiTheme.Padding),
            BackColor = UiTheme.Background,
            Tag = "theme-bg"
        };

        _topNav.NavigationRequested += OnNavigationRequested;
        Controls.Add(ContentPanel);
        Controls.Add(_topNav);
        Controls.Add(menu);

        UiTheme.Changed += OnThemeChanged;
        FormClosed += (_, _) =>
        {
            UiTheme.Changed -= OnThemeChanged;
            NavigationService.Unregister(this);
        };
    }

    private void SetTheme(AppTheme theme)
    {
        UiTheme.SetTheme(theme);
    }

    private void OnThemeChanged(object? sender, EventArgs e)
    {
        _lightThemeItem.Checked = UiTheme.Current == AppTheme.Light;
        _darkThemeItem.Checked = UiTheme.Current == AppTheme.Dark;
        NavigationService.RefreshAllThemes();
    }

    public void ApplyTheme()
    {
        BackColor = UiTheme.Background;
        ContentPanel.BackColor = UiTheme.Background;
        if (MainMenuStrip != null)
            UiTheme.StyleMenuStrip(MainMenuStrip);
        _topNav.ApplyTheme();
        UiTheme.ApplyThemeToControl(ContentPanel);
        Invalidate(true);
    }

    public void SetActiveNavKey(string key) => _topNav.SetActiveNavKey(key);

    protected virtual void OnNavigationRequested(object? sender, string key)
    {
        var target = CreateFormForNavKey(key);
        if (target != null)
            NavigationService.NavigateTo(target, key);
    }

    protected virtual Form? CreateFormForNavKey(string key) => key switch
    {
        "home" => new MainForm(),
        "students" => new StudentListForm(),
        "disciplines" => new DisciplineListForm(),
        "grades" => new GradeListForm(),
        "grade-entry" => new GradeEntryForm(),
        "group-grades" => new GroupGradesForm(),
        "debtors" => new DebtorsListForm(),
        "semester-report" => new SemesterPerformanceReportForm(),
        "discipline-report" => new DisciplineSummaryReportForm(),
        _ => null
    };
}
