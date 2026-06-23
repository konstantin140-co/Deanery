using Deanery.Data.Context;
using Deanery.Data.Models;
using Deanery.Forms.Ui;

namespace Deanery.Forms.Forms;

public class DisciplineEditForm : Form
{
    private readonly AppDbContext _db = new();
    private readonly int _id;
    private readonly TextBox _txtName = new() { Width = 280 };
    private readonly TextBox _txtTeacher = new() { Width = 280 };
    private readonly NumericUpDown _numSemester = new() { Width = 120, Minimum = 1, Maximum = 12, Value = 1 };
    private readonly NumericUpDown _numHours = new() { Width = 120, Minimum = 1, Maximum = 500, Value = 72 };

    public DisciplineEditForm(int id)
    {
        _id = id;
        Text = id == 0 ? "Добавление дисциплины" : "Редактирование дисциплины";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(460, 280);
        BackColor = UiTheme.Background;
        Font = UiTheme.BodyFont;

        var table = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(16), RowCount = 4 };
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        AddRow(table, 0, "Название:", _txtName);
        AddRow(table, 1, "Преподаватель:", _txtTeacher);
        AddRow(table, 2, "Семестр:", _numSemester);
        AddRow(table, 3, "Часов:", _numHours);

        var btnSave = new Button { Text = "Сохранить" };
        var btnCancel = new Button { Text = "Отмена", DialogResult = DialogResult.Cancel };
        UiTheme.StylePrimaryButton(btnSave);
        UiTheme.StyleSecondaryButton(btnCancel);
        btnSave.Click += (_, _) => Save();

        var buttons = new FlowLayoutPanel { Dock = DockStyle.Bottom, FlowDirection = FlowDirection.RightToLeft, Height = 48, Padding = new Padding(8) };
        buttons.Controls.Add(btnCancel);
        buttons.Controls.Add(btnSave);

        Controls.Add(table);
        Controls.Add(buttons);
        FormClosed += (_, _) => _db.Dispose();
        UiTheme.ApplyThemeToControl(this);

        if (_id != 0)
        {
            var item = _db.Disciplines.Find(_id);
            if (item != null)
            {
                _txtName.Text = item.Name;
                _txtTeacher.Text = item.Teacher;
                _numSemester.Value = item.Semester;
                _numHours.Value = item.TotalHours;
            }
        }
    }

    private static void AddRow(TableLayoutPanel table, int row, string label, Control control)
    {
        table.Controls.Add(new Label { Text = label, AutoSize = true, ForeColor = UiTheme.TextSecondary, Font = UiTheme.BodyFont }, 0, row);
        table.Controls.Add(control is TextBox tb ? UiTheme.WrapInput(tb) : control, 1, row);
    }

    private void Save()
    {
        FormValidation.ResetBackColor(_txtName, _txtTeacher);
        var valid = FormValidation.RequireText(_txtName)
                    & FormValidation.RequireText(_txtTeacher);
        if (!valid)
        {
            MessageBox.Show("Заполните обязательные поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            Discipline item = _id == 0
                ? new Discipline()
                : _db.Disciplines.Find(_id) ?? throw new InvalidOperationException("Запись не найдена");

            item.Name = _txtName.Text.Trim();
            item.Teacher = _txtTeacher.Text.Trim();
            item.Semester = (int)_numSemester.Value;
            item.TotalHours = (int)_numHours.Value;

            if (_id == 0) _db.Disciplines.Add(item);
            _db.SaveChanges();
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Ошибка при сохранении: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
