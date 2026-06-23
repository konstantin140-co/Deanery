using Deanery.Data.Constants;
using Deanery.Data.Context;
using Deanery.Data.Models;
using Deanery.Forms.Ui;

namespace Deanery.Forms.Forms;

public class StudentEditForm : Form
{
    private readonly AppDbContext _db = new();
    private readonly int _id;
    private readonly TextBox _txtLastName = new() { Width = 280 };
    private readonly TextBox _txtFirstName = new() { Width = 280 };
    private readonly TextBox _txtMiddleName = new() { Width = 280 };
    private readonly TextBox _txtGroup = new() { Width = 280 };
    private readonly TextBox _txtPhone = new() { Width = 280 };
    private readonly ComboBox _cmbStatus = new() { Width = 280, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly DateTimePicker _dtpEnrollment = new() { Width = 280, Format = DateTimePickerFormat.Short };

    public StudentEditForm(int id)
    {
        _id = id;
        Text = id == 0 ? "Добавление студента" : "Редактирование студента";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(460, 380);
        BackColor = UiTheme.Background;
        Font = UiTheme.BodyFont;

        _cmbStatus.Items.AddRange(StudentStatuses.All);

        var table = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(16), RowCount = 7 };
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        AddRow(table, 0, "Фамилия:", _txtLastName);
        AddRow(table, 1, "Имя:", _txtFirstName);
        AddRow(table, 2, "Отчество:", _txtMiddleName);
        AddRow(table, 3, "Группа:", _txtGroup);
        AddRow(table, 4, "Телефон:", _txtPhone);
        AddRow(table, 5, "Статус:", _cmbStatus);
        AddRow(table, 6, "Дата зачисления:", _dtpEnrollment);

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
            var item = _db.Students.Find(_id);
            if (item != null)
            {
                _txtLastName.Text = item.LastName;
                _txtFirstName.Text = item.FirstName;
                _txtMiddleName.Text = item.MiddleName;
                _txtGroup.Text = item.GroupName;
                _txtPhone.Text = item.Phone;
                _cmbStatus.SelectedItem = item.Status;
                _dtpEnrollment.Value = item.EnrollmentDate;
            }
        }
        else
        {
            _cmbStatus.SelectedIndex = 0;
            _dtpEnrollment.Value = DateTime.Today;
        }
    }

    private static void AddRow(TableLayoutPanel table, int row, string label, Control control)
    {
        table.Controls.Add(new Label { Text = label, AutoSize = true, ForeColor = UiTheme.TextSecondary, Font = UiTheme.BodyFont }, 0, row);
        table.Controls.Add(control is TextBox tb ? UiTheme.WrapInput(tb) : control, 1, row);
    }

    private void Save()
    {
        FormValidation.ResetBackColor(_txtLastName, _txtFirstName, _txtGroup, _txtPhone);
        var valid = FormValidation.RequireText(_txtLastName)
                    & FormValidation.RequireText(_txtFirstName)
                    & FormValidation.RequireText(_txtGroup)
                    & FormValidation.RequireText(_txtPhone);
        if (!valid)
        {
            MessageBox.Show("Заполните обязательные поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (_cmbStatus.SelectedItem == null)
        {
            MessageBox.Show("Выберите статус.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            Student item = _id == 0
                ? new Student()
                : _db.Students.Find(_id) ?? throw new InvalidOperationException("Запись не найдена");

            item.LastName = _txtLastName.Text.Trim();
            item.FirstName = _txtFirstName.Text.Trim();
            item.MiddleName = _txtMiddleName.Text.Trim();
            item.GroupName = _txtGroup.Text.Trim();
            item.Phone = _txtPhone.Text.Trim();
            item.Status = _cmbStatus.SelectedItem.ToString()!;
            item.EnrollmentDate = _dtpEnrollment.Value.Date;

            if (_id == 0) _db.Students.Add(item);
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
