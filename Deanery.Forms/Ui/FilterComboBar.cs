namespace Deanery.Forms.Ui;

internal class FilterComboBar : Panel, IThemedControl
{
    private readonly ComboBox _semesterCombo = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 160 };
    private readonly ComboBox _groupCombo = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 160 };

    public event EventHandler? FilterChanged;

    public ComboBox SemesterCombo => _semesterCombo;
    public ComboBox GroupCombo => _groupCombo;

    public string? SelectedSemester => _semesterCombo.SelectedItem as string;
    public string? SelectedGroup => _groupCombo.SelectedItem as string;

    public FilterComboBar()
    {
        Height = 44;
        Dock = DockStyle.Top;
        BackColor = Color.Transparent;
        Padding = new Padding(0, 4, 0, 8);

        var layout = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoSize = true,
            BackColor = Color.Transparent
        };

        layout.Controls.Add(CreateField("Семестр:", _semesterCombo));
        layout.Controls.Add(CreateField("Группа:", _groupCombo));

        _semesterCombo.SelectedIndexChanged += (_, _) => FilterChanged?.Invoke(this, EventArgs.Empty);
        _groupCombo.SelectedIndexChanged += (_, _) => FilterChanged?.Invoke(this, EventArgs.Empty);

        Controls.Add(layout);
        ApplyTheme();
    }

    public void ApplyTheme()
    {
        BackColor = Color.Transparent;
        _semesterCombo.BackColor = UiTheme.CardBackground;
        _semesterCombo.ForeColor = UiTheme.TextPrimary;
        _groupCombo.BackColor = UiTheme.CardBackground;
        _groupCombo.ForeColor = UiTheme.TextPrimary;
    }

    public void SetSemesters(IEnumerable<string> semesters, string? selected = null)
    {
        _semesterCombo.Items.Clear();
        foreach (var semester in semesters)
            _semesterCombo.Items.Add(semester);
        SelectItem(_semesterCombo, selected);
    }

    public void SetGroups(IEnumerable<string> groups, string? selected = null)
    {
        _groupCombo.Items.Clear();
        foreach (var group in groups)
            _groupCombo.Items.Add(group);
        SelectItem(_groupCombo, selected);
    }

    private static void SelectItem(ComboBox combo, string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            if (combo.Items.Count > 0)
                combo.SelectedIndex = 0;
            return;
        }

        var index = combo.Items.IndexOf(value);
        combo.SelectedIndex = index >= 0 ? index : combo.Items.Count > 0 ? 0 : -1;
    }

    private static Control CreateField(string labelText, ComboBox combo)
    {
        var label = new Label
        {
            Text = labelText,
            AutoSize = true,
            Font = UiTheme.BodyFont,
            ForeColor = UiTheme.TextSecondary,
            Margin = new Padding(0, 8, 8, 0),
            Tag = "secondary"
        };
        combo.Margin = new Padding(0, 4, 24, 0);
        combo.Font = UiTheme.BodyFont;

        var panel = new FlowLayoutPanel
        {
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            BackColor = Color.Transparent,
            Margin = new Padding(0)
        };
        panel.Controls.Add(label);
        panel.Controls.Add(combo);
        return panel;
    }
}
