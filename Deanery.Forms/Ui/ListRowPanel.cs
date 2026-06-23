namespace Deanery.Forms.Ui;

internal class ListRowPanel : Panel, IThemedControl
{
    private readonly FlowLayoutPanel _flow = new()
    {
        Dock = DockStyle.Top,
        FlowDirection = FlowDirection.TopDown,
        WrapContents = false,
        AutoSize = true,
        AutoSizeMode = AutoSizeMode.GrowAndShrink,
        BackColor = Color.Transparent,
        Padding = new Padding(0)
    };

    public ListRowPanel()
    {
        Dock = DockStyle.Fill;
        AutoScroll = true;
        BackColor = Color.Transparent;
        Controls.Add(_flow);
    }

    public IReadOnlyList<ListRowControl> Rows =>
        _flow.Controls.OfType<ListRowControl>().ToList();

    public void ApplyTheme()
    {
        BackColor = Color.Transparent;
        _flow.BackColor = Color.Transparent;
        foreach (var row in Rows)
            row.ApplyTheme();
    }

    public ListRowControl AddRow(int id, string title, string status, string details)
    {
        var row = CreateRow();
        row.Bind(id, title, status, details);
        return row;
    }

    public ListRowControl AddRowWithScore(int id, string title, int score, string details)
    {
        var row = CreateRow();
        row.BindWithScore(id, title, score, details);
        return row;
    }

    public void ClearRows()
    {
        _flow.Controls.Clear();
    }

    private ListRowControl CreateRow()
    {
        var row = new ListRowControl { Width = Math.Max(400, ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 4) };
        row.EditClicked += (_, id) => RowEditClicked?.Invoke(this, id);
        row.DeleteClicked += (_, id) => RowDeleteClicked?.Invoke(this, id);
        _flow.Controls.Add(row);
        return row;
    }

    public event EventHandler<int>? RowEditClicked;
    public event EventHandler<int>? RowDeleteClicked;

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        var width = Math.Max(400, ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 4);
        foreach (Control control in _flow.Controls)
            control.Width = width;
    }
}
