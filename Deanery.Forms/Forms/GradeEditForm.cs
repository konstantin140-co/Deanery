using Deanery.Data.Constants;
using Deanery.Data.Context;
using Deanery.Data.Models;
using Deanery.Forms.Ui;
using Microsoft.EntityFrameworkCore;

namespace Deanery.Forms.Forms;

public class GradeEditForm : Form
{
    private readonly AppDbContext _db = new();
    private readonly int _id;
    private readonly ComboBox _cmbStudent = new() { Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _cmbDiscipline = new() { Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _cmbWorkType = new() { Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly DateTimePicker _dtpDate = new() { Width = 300, Format = DateTimePickerFormat.Short };
    private readonly NumericUpDown _numScore = new() { Width = 120, Minimum = 2, Maximum = 5, Value = 4 };

    public GradeEditForm(int id)
    {
        _id = id;
        Text = id == 0 ? "Добавление оценки" : "Редактирование оценки";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(480, 320);
        BackColor = UiTheme.Background;
        Font = UiTheme.BodyFont;

        _cmbWorkType.Items.AddRange(WorkTypes.All);

        var table = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(16), RowCount = 5 };
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        AddRow(table, 0, "Студент:", _cmbStudent);
        AddRow(table, 1, "Дисциплина:", _cmbDiscipline);
        AddRow(table, 2, "Вид работы:", _cmbWorkType);
        AddRow(table, 3, "Дата:", _dtpDate);
        AddRow(table, 4, "Оценка:", _numScore);

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

        LoadCombos();

        if (_id != 0)
        {
            var item = _db.Grades.Find(_id);
            if (item != null)
            {
                _cmbStudent.SelectedValue = item.StudentId;
                _cmbDiscipline.SelectedValue = item.DisciplineId;
                _cmbWorkType.SelectedItem = item.WorkType;
                _dtpDate.Value = item.Date;
                _numScore.Value = item.Score;
            }
        }
        else
        {
            _cmbWorkType.SelectedIndex = 0;
            _dtpDate.Value = DateTime.Today;
        }
    }

    private static void AddRow(TableLayoutPanel table, int row, string label, Control control)
    {
        table.Controls.Add(new Label { Text = label, AutoSize = true, ForeColor = UiTheme.TextSecondary, Font = UiTheme.BodyFont }, 0, row);
        table.Controls.Add(control, 1, row);
    }

    private void LoadCombos()
    {
        var students = _db.Students.OrderBy(s => s.LastName).ThenBy(s => s.FirstName).ToList();
        _cmbStudent.DisplayMember = "FullName";
        _cmbStudent.ValueMember = "Id";
        _cmbStudent.DataSource = students;

        var disciplines = _db.Disciplines.OrderBy(d => d.Name).ToList();
        _cmbDiscipline.DisplayMember = "Name";
        _cmbDiscipline.ValueMember = "Id";
        _cmbDiscipline.DataSource = disciplines;
    }

    private void Save()
    {
        if (_cmbStudent.SelectedValue is not int studentId ||
            _cmbDiscipline.SelectedValue is not int disciplineId ||
            _cmbWorkType.SelectedItem == null)
        {
            MessageBox.Show("Заполните все поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            Grade item = _id == 0
                ? new Grade()
                : _db.Grades.Find(_id) ?? throw new InvalidOperationException("Запись не найдена");

            item.StudentId = studentId;
            item.DisciplineId = disciplineId;
            item.WorkType = _cmbWorkType.SelectedItem.ToString()!;
            item.Date = _dtpDate.Value.Date;
            item.Score = (int)_numScore.Value;

            if (_id == 0) _db.Grades.Add(item);
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
