namespace Deanery.Forms.Ui;

internal enum AppTheme
{
    Light,
    Dark
}

internal static class UiTheme
{
    public static AppTheme Current { get; private set; } = AppTheme.Light;
    public static event EventHandler? Changed;

    public static Color Background => Current == AppTheme.Light
        ? Color.FromArgb(245, 247, 250)
        : Color.FromArgb(17, 24, 39);

    public static Color NavBackground => Current == AppTheme.Light
        ? Color.White
        : Color.FromArgb(31, 41, 55);

    public static Color CardBackground => Current == AppTheme.Light
        ? Color.White
        : Color.FromArgb(55, 65, 81);

    public static Color Accent => Color.FromArgb(37, 99, 235);

    public static Color AccentHover => Color.FromArgb(29, 78, 216);

    public static Color TextPrimary => Current == AppTheme.Light
        ? Color.FromArgb(17, 24, 39)
        : Color.FromArgb(249, 250, 251);

    public static Color TextSecondary => Current == AppTheme.Light
        ? Color.FromArgb(107, 114, 128)
        : Color.FromArgb(156, 163, 175);

    public static Color Border => Current == AppTheme.Light
        ? Color.FromArgb(229, 231, 235)
        : Color.FromArgb(75, 85, 99);

    public static Color NavActive => Current == AppTheme.Light
        ? Color.FromArgb(239, 246, 255)
        : Color.FromArgb(30, 58, 138);

    public static Color NavActiveBar => Accent;

    public static (Color Top, Color Bottom) NavHoverGradient => Current == AppTheme.Light
        ? (Color.FromArgb(80, 219, 234, 254), Color.FromArgb(50, 191, 219, 254))
        : (Color.FromArgb(65, 30, 58, 138), Color.FromArgb(45, 23, 37, 84));

    public static (Color Top, Color Bottom) NavActiveGradient => Current == AppTheme.Light
        ? (Color.FromArgb(130, 219, 234, 254), Color.FromArgb(80, 147, 197, 253))
        : (Color.FromArgb(70, 37, 99, 235), Color.FromArgb(45, 29, 78, 216));

    public static Color MenuBackground => Current == AppTheme.Light
        ? Color.FromArgb(250, 250, 250)
        : Color.FromArgb(31, 41, 55);

    public static Color MenuForeground => TextPrimary;

    public static readonly Font TitleFont = new("Segoe UI", 20F, FontStyle.Bold);
    public static readonly Font SectionFont = new("Segoe UI", 9F, FontStyle.Bold);
    public static readonly Font BodyFont = new("Segoe UI", 9.5F);
    public static readonly Font SmallFont = new("Segoe UI", 8.5F);

    public const int NavHeight = 48;
    public const int CornerRadius = 12;
    public const int Padding = 24;

    public static void SetTheme(AppTheme theme)
    {
        if (Current == theme) return;
        Current = theme;
        Changed?.Invoke(null, EventArgs.Empty);
    }

    public static void StylePrimaryButton(Button button)
    {
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 0;
        button.BackColor = Accent;
        button.ForeColor = Color.White;
        button.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        button.Cursor = Cursors.Hand;
        button.Height = 36;
        button.Padding = new Padding(12, 0, 12, 0);
    }

    public static void StyleSecondaryButton(Button button)
    {
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderColor = Border;
        button.FlatAppearance.BorderSize = 1;
        button.BackColor = CardBackground;
        button.ForeColor = TextPrimary;
        button.Font = BodyFont;
        button.Cursor = Cursors.Hand;
        button.Height = 32;
    }

    public static void StyleTextBox(TextBox textBox)
    {
        textBox.BorderStyle = BorderStyle.None;
        textBox.BackColor = CardBackground;
        textBox.ForeColor = TextPrimary;
        textBox.Font = BodyFont;
    }

    public static void StyleMenuStrip(MenuStrip menu)
    {
        menu.BackColor = MenuBackground;
        menu.ForeColor = MenuForeground;
        menu.Renderer = new ToolStripProfessionalRenderer(new MenuColorTable());
    }

    public static RoundedPanel WrapInput(Control input, int width = 300)
    {
        if (input is TextBox textBox)
            StyleTextBox(textBox);

        input.Dock = DockStyle.Fill;
        return new RoundedPanel
        {
            Width = width,
            Height = 34,
            Padding = new Padding(10, 6, 10, 6),
            Controls = { input }
        };
    }

    public static void StyleDataGrid(DataGridView grid)
    {
        grid.BackgroundColor = CardBackground;
        grid.BorderStyle = BorderStyle.None;
        grid.EnableHeadersVisualStyles = false;
        grid.ColumnHeadersDefaultCellStyle.BackColor = Background;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = TextSecondary;
        grid.ColumnHeadersDefaultCellStyle.Font = SectionFont;
        grid.DefaultCellStyle.BackColor = CardBackground;
        grid.DefaultCellStyle.ForeColor = TextPrimary;
        grid.DefaultCellStyle.SelectionBackColor = Current == AppTheme.Light
            ? Color.FromArgb(219, 234, 254)
            : Color.FromArgb(30, 58, 138);
        grid.DefaultCellStyle.SelectionForeColor = TextPrimary;
        grid.GridColor = Border;
        grid.RowHeadersVisible = false;
    }

    public static void ApplyThemeToControl(Control control)
    {
        if (control is IThemedControl themed)
            themed.ApplyTheme();

        switch (control)
        {
            case Form form:
                form.BackColor = Background;
                form.ForeColor = TextPrimary;
                break;
            case MenuStrip menu:
                StyleMenuStrip(menu);
                break;
            case Panel { Tag: "theme-bg" }:
                control.BackColor = Background;
                break;
            case Label label when label.Tag as string == "primary":
                label.ForeColor = TextPrimary;
                break;
            case Label label when label.Tag as string == "secondary":
                label.ForeColor = TextSecondary;
                break;
            case Label label when label.Tag as string == "accent":
                label.ForeColor = Accent;
                break;
            case Button button when IsInsideFilterTabs(button):
                break;
            case Button button:
                if (button.BackColor == Accent || (button.FlatAppearance.BorderSize == 0 && button.ForeColor == Color.White))
                    StylePrimaryButton(button);
                else
                    StyleSecondaryButton(button);
                break;
            case TextBox textBox:
                StyleTextBox(textBox);
                break;
            case DataGridView grid:
                StyleDataGrid(grid);
                break;
            case ComboBox combo:
                combo.BackColor = CardBackground;
                combo.ForeColor = TextPrimary;
                break;
            case NumericUpDown numeric:
                numeric.BackColor = CardBackground;
                numeric.ForeColor = TextPrimary;
                break;
            case DateTimePicker picker:
                picker.CalendarMonthBackground = CardBackground;
                picker.ForeColor = TextPrimary;
                break;
        }

        foreach (Control child in control.Controls)
            ApplyThemeToControl(child);
    }

    private static bool IsInsideFilterTabs(Control control)
    {
        for (var c = control.Parent; c != null; c = c.Parent)
        {
            if (c.GetType().Name == "FilterTabsControl")
                return true;
        }

        return false;
    }

    private sealed class MenuColorTable : ProfessionalColorTable
    {
        public override Color MenuItemSelected => UiTheme.NavActive;
        public override Color MenuItemSelectedGradientBegin => UiTheme.NavActive;
        public override Color MenuItemSelectedGradientEnd => UiTheme.NavActive;
        public override Color MenuItemBorder => UiTheme.Border;
        public override Color MenuBorder => UiTheme.Border;
        public override Color ToolStripDropDownBackground => UiTheme.CardBackground;
        public override Color ImageMarginGradientBegin => UiTheme.CardBackground;
        public override Color ImageMarginGradientMiddle => UiTheme.CardBackground;
        public override Color ImageMarginGradientEnd => UiTheme.CardBackground;
    }
}
