using System.Drawing.Drawing2D;

namespace Deanery.Forms.Ui;

internal class TopNavBar : Panel, IThemedControl
{
    private readonly Dictionary<string, NavTabItemControl> _navItems = new();
    private readonly FlowLayoutPanel _flow = new()
    {
        Dock = DockStyle.Fill,
        WrapContents = false,
        AutoScroll = true,
        FlowDirection = FlowDirection.LeftToRight,
        Padding = new Padding(8, 6, 8, 6)
    };

    private string? _activeKey;

    public event EventHandler<string>? NavigationRequested;

    public TopNavBar()
    {
        Dock = DockStyle.Top;
        Height = UiTheme.NavHeight;
        Controls.Add(_flow);

        AddNavItem("Главная", "home");
        AddNavItem("Студенты", "students");
        AddNavItem("Дисциплины", "disciplines");
        AddNavItem("Оценки", "grades");
        AddNavItem("Ввод оценок", "grade-entry");
        AddNavItem("Оценки группы", "group-grades");
        AddNavItem("Должники", "debtors");
        AddNavItem("Отчёт за семестр", "semester-report");
        AddNavItem("Отчёт по дисциплине", "discipline-report");

        ApplyTheme();
    }

    public void ApplyTheme()
    {
        BackColor = UiTheme.NavBackground;
        _flow.BackColor = UiTheme.NavBackground;
        foreach (var item in _navItems.Values)
            item.ApplyTheme();
        Invalidate();
    }

    public void SetActiveNavKey(string key)
    {
        _activeKey = key;
        foreach (var (navKey, item) in _navItems)
            item.SetActive(navKey == key);
    }

    private void AddNavItem(string text, string key)
    {
        var item = new NavTabItemControl(text, key)
        {
            Margin = new Padding(0, 0, 4, 0)
        };
        item.ItemClicked += (_, _) =>
        {
            SetActiveNavKey(key);
            NavigationRequested?.Invoke(this, key);
        };
        _navItems[key] = item;
        _flow.Controls.Add(item);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        using var pen = new Pen(UiTheme.Border);
        e.Graphics.DrawLine(pen, 0, Height - 1, Width, Height - 1);
    }

    private sealed class NavTabItemControl : Control, IThemedControl
    {
        private bool _hover;
        private bool _active;

        public string NavKey { get; }

        public event EventHandler? ItemClicked;

        public NavTabItemControl(string text, string navKey)
        {
            NavKey = navKey;
            Text = text;
            AutoSize = true;
            MinimumSize = new Size(80, 34);
            Padding = new Padding(14, 6, 14, 6);
            Cursor = Cursors.Hand;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            ApplyTheme();
        }

        public void SetActive(bool active)
        {
            _active = active;
            Font = _active ? new Font(UiTheme.BodyFont, FontStyle.Bold) : UiTheme.BodyFont;
            Invalidate();
        }

        public void ApplyTheme()
        {
            Font = _active ? new Font(UiTheme.BodyFont, FontStyle.Bold) : UiTheme.BodyFont;
            Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _hover = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _hover = false;
            Invalidate();
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            ItemClicked?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            var textSize = TextRenderer.MeasureText(Text, Font);
            var width = Math.Max(MinimumSize.Width, textSize.Width + Padding.Horizontal);
            var height = Math.Max(MinimumSize.Height, textSize.Height + Padding.Vertical);
            if (Width != width || Height != height)
                Size = new Size(width, height);

            var rect = new Rectangle(2, 2, Width - 4, Height - 4);

            if (_active || _hover)
            {
                using var path = RoundRectHelper.CreatePath(rect, 10);
                var (top, bottom) = _active ? UiTheme.NavActiveGradient : UiTheme.NavHoverGradient;
                using var brush = new LinearGradientBrush(rect, top, bottom, LinearGradientMode.Vertical);
                e.Graphics.FillPath(brush, path);
            }

            var textColor = _active ? UiTheme.Accent : UiTheme.TextPrimary;
            var textRect = new Rectangle(Padding.Left, 0, Width - Padding.Horizontal, Height);
            TextRenderer.DrawText(e.Graphics, Text, Font, textRect, textColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }
    }
}
