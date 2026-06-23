namespace Deanery.Forms.Ui;

internal class ListRowControl : RoundedPanel, IThemedControl
{
    private readonly Panel _strip = new() { Width = 4, Dock = DockStyle.Left };
    private readonly Label _title = new()
    {
        AutoSize = true,
        Font = new Font("Segoe UI", 11F, FontStyle.Bold),
        MaximumSize = new Size(0, 0)
    };
    private readonly StatusBadgeControl _badge = new();
    private readonly Label _details = new()
    {
        AutoSize = true,
        Font = UiTheme.SmallFont,
        MaximumSize = new Size(0, 0),
        Margin = new Padding(0, 4, 0, 0)
    };
    private readonly Button _editButton = new() { Text = "Редактировать", AutoSize = true };
    private readonly Button _deleteButton = new() { Text = "Удалить", AutoSize = true };

    public int EntityId { get; private set; }

    public event EventHandler<int>? EditClicked;
    public event EventHandler<int>? DeleteClicked;

    public ListRowControl()
    {
        Height = 72;
        Dock = DockStyle.Top;
        Margin = new Padding(0, 0, 0, 8);
        Padding = new Padding(0);
        Radius = UiTheme.CornerRadius;

        _title.Tag = "primary";
        _details.Tag = "secondary";

        var content = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(16, 12, 16, 12),
            BackColor = Color.Transparent
        };

        var infoPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoSize = true,
            BackColor = Color.Transparent,
            Margin = new Padding(12, 0, 0, 0)
        };

        var titleRow = new FlowLayoutPanel
        {
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            BackColor = Color.Transparent,
            Margin = new Padding(0)
        };
        titleRow.Controls.Add(_title);
        titleRow.Controls.Add(_badge);
        _badge.Margin = new Padding(12, 2, 0, 0);

        infoPanel.Controls.Add(titleRow);
        infoPanel.Controls.Add(_details);

        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Right,
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            BackColor = Color.Transparent,
            Padding = new Padding(0, 16, 0, 0)
        };
        UiTheme.StyleSecondaryButton(_editButton);
        UiTheme.StyleSecondaryButton(_deleteButton);
        _editButton.Margin = new Padding(0, 0, 8, 0);
        _editButton.Click += (_, _) => EditClicked?.Invoke(this, EntityId);
        _deleteButton.Click += (_, _) => DeleteClicked?.Invoke(this, EntityId);
        actions.Controls.Add(_editButton);
        actions.Controls.Add(_deleteButton);

        content.Controls.Add(actions);
        content.Controls.Add(infoPanel);

        Controls.Add(content);
        Controls.Add(_strip);

        ApplyTheme();
    }

    public new void ApplyTheme()
    {
        base.ApplyTheme();
        _title.ForeColor = UiTheme.TextPrimary;
        _details.ForeColor = UiTheme.TextSecondary;
        UiTheme.StyleSecondaryButton(_editButton);
        UiTheme.StyleSecondaryButton(_deleteButton);
        if (!string.IsNullOrEmpty(_badge.Text))
            _badge.SetStatus(_badge.Text);
        UpdateStripColor();
    }

    public void Bind(int id, string title, string status, string details,
        string editText = "Редактировать", string deleteText = "Удалить")
    {
        EntityId = id;
        _title.Text = title;
        _badge.SetStatus(status);
        _details.Text = details;
        _editButton.Text = editText;
        _deleteButton.Text = deleteText;
        UpdateStripColor();
    }

    public void BindWithScore(int id, string title, int score, string details,
        string editText = "Редактировать", string deleteText = "Удалить")
    {
        EntityId = id;
        _title.Text = title;
        _badge.SetScore(score);
        _details.Text = details;
        _editButton.Text = editText;
        _deleteButton.Text = deleteText;
        UpdateStripColor(score);
    }

    private void UpdateStripColor(int? score = null)
    {
        _strip.BackColor = score switch
        {
            >= 5 => Color.FromArgb(34, 197, 94),
            4 => UiTheme.Accent,
            3 => Color.FromArgb(234, 179, 8),
            2 => Color.FromArgb(239, 68, 68),
            _ => GetStripColorByStatus(_badge.Text)
        };
    }

    private static Color GetStripColorByStatus(string status) => status switch
    {
        "Обучается" or "Зачёт" or "Отлично" => Color.FromArgb(34, 197, 94),
        "Хорошо" => UiTheme.Accent,
        "Удовл." or "Удовлетворительно" => Color.FromArgb(234, 179, 8),
        "Неуд." or "Неудовлетворительно" or "Должник" or "Отчислен" => Color.FromArgb(239, 68, 68),
        "Академ" => Color.FromArgb(168, 85, 247),
        _ => UiTheme.Border
    };
}
