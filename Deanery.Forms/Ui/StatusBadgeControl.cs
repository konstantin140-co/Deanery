namespace Deanery.Forms.Ui;

internal class StatusBadgeControl : Label
{
    public StatusBadgeControl()
    {
        AutoSize = false;
        TextAlign = ContentAlignment.MiddleCenter;
        Font = UiTheme.SmallFont;
        Height = 24;
        Width = 90;
        Padding = new Padding(8, 2, 8, 2);
    }

    public void SetStatus(string status)
    {
        Text = status;
        var (back, fore) = status switch
        {
            "Обучается" or "Зачёт" or "Отлично" => (Color.FromArgb(220, 252, 231), Color.FromArgb(22, 101, 52)),
            "Хорошо" => (Color.FromArgb(219, 234, 254), Color.FromArgb(30, 64, 175)),
            "Удовл." or "Удовлетворительно" => (Color.FromArgb(254, 243, 199), Color.FromArgb(146, 64, 14)),
            "Неуд." or "Неудовлетворительно" or "Должник" or "Отчислен" => (Color.FromArgb(254, 226, 226), Color.FromArgb(153, 27, 27)),
            "Академ" => (Color.FromArgb(243, 232, 255), Color.FromArgb(107, 33, 168)),
            "Неявка" => (Color.FromArgb(243, 244, 246), Color.FromArgb(75, 85, 99)),
            _ => (Color.FromArgb(243, 244, 246), UiTheme.TextSecondary)
        };
        BackColor = back;
        ForeColor = fore;
        Width = Math.Max(90, TextRenderer.MeasureText(status, Font).Width + Padding.Horizontal + 4);
    }

    public void SetScore(int score)
    {
        var status = score switch
        {
            >= 5 => "Отлично",
            4 => "Хорошо",
            3 => "Удовл.",
            2 => "Неуд.",
            _ => "Неявка"
        };
        SetStatus(status);
    }
}
