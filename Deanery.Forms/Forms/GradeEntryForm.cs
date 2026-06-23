using Deanery.Data.Constants;
using Deanery.Data.Context;
using Deanery.Data.Models;
using Deanery.Forms.Ui;
using Microsoft.EntityFrameworkCore;

namespace Deanery.Forms.Forms;

public class GradeEntryForm : TopNavShellForm
{
    private readonly AppDbContext _db = new();
    private readonly ComboBox _cmbStudent = new() { Width = 360, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _cmbDiscipline = new() { Width = 360, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _cmbWorkType = new() { Width = 360, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly DateTimePicker _dtpDate = new() { Width = 220, Format = DateTimePickerFormat.Short };
    private readonly NumericUpDown _numScore = new() { Width = 120, Minimum = 2, Maximum = 5, Value = 4 };

    public GradeEntryForm()
    {
        SetActiveNavKey("grade-entry");
        NavigationService.NavigateTo(this, "grade-entry");

        var header = new PageHeaderControl();
        header.SetTitle("Ввод оценок", "Регистрация новой оценки");
        header.ConfigureAction("", false);
        header.HideSearch();

        _cmbWorkType.Items.AddRange(WorkTypes.All);
        _cmbWorkType.SelectedIndex = 0;
        _dtpDate.Value = DateTime.Today;

        var card = new RoundedPanel { Dock = DockStyle.Top, Height = 300, Padding = new Padding(24) };
        var table = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2 };
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        AddRow(table, 0, "Студент:", _cmbStudent);
        AddRow(table, 1, "Дисциплина:", _cmbDiscipline);
        AddRow(table, 2, "Вид работы:", _cmbWorkType);
        AddRow(table, 3, "Дата:", _dtpDate);
        AddRow(table, 4, "Оценка:", _numScore);
        card.Controls.Add(table);

        var btnSave = new Button { Text = "Сохранить оценку", Width = 160 };
        UiTheme.StylePrimaryButton(btnSave);
        btnSave.Click += (_, _) => SaveGrade();

        var actions = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 48, Padding = new Padding(0, 8, 0, 0) };
        actions.Controls.Add(btnSave);

        ContentPanel.Controls.Add(actions);
        ContentPanel.Controls.Add(card);
        ContentPanel.Controls.Add(header);

        Load += (_, _) => LoadCombos();
        FormClosed += (_, _) => _db.Dispose();
        ApplyTheme();
    }

    private static void AddRow(TableLayoutPanel table, int row, string label, Control control)
    {
        table.Controls.Add(new Label { Text = label, AutoSize = true, ForeColor = UiTheme.TextSecondary, Font = UiTheme.BodyFont }, 0, row);
        table.Controls.Add(control, 1, row);
    }

    private void LoadCombos()
    {
        var students = _db.Students.OrderBy(s => s.LastName).ThenBy(s => s.FirstName).ToList();
        if (students.Count == 0)
            MessageBox.Show("Нет студентов в базе.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);

        _cmbStudent.DisplayMember = "FullName";
        _cmbStudent.ValueMember = "Id";
        _cmbStudent.DataSource = students;

        var disciplines = _db.Disciplines.OrderBy(d => d.Name).ToList();
        if (disciplines.Count == 0)
            MessageBox.Show("Нет дисциплин в базе.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);

        _cmbDiscipline.DisplayMember = "Name";
        _cmbDiscipline.ValueMember = "Id";
        _cmbDiscipline.DataSource = disciplines;
    }

    private void SaveGrade()
    {
        try
        {
            if (_cmbStudent.SelectedValue is not int studentId)
                throw new ArgumentException("Выберите студента");
            if (_cmbDiscipline.SelectedValue is not int disciplineId)
                throw new ArgumentException("Выберите дисциплину");
            if (_cmbWorkType.SelectedItem == null)
                throw new ArgumentException("Выберите вид работы");

            var grade = new Grade
            {
                StudentId = studentId,
                DisciplineId = disciplineId,
                WorkType = _cmbWorkType.SelectedItem.ToString()!,
                Date = _dtpDate.Value.Date,
                Score = (int)_numScore.Value
            };

            _db.Grades.Add(grade);
            _db.SaveChanges();

            MessageBox.Show("Оценка успешно сохранена.", "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
            _numScore.Value = 4;
            _dtpDate.Value = DateTime.Today;
        }
        catch (ArgumentException ex)
        {
            MessageBox.Show(ex.Message, "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
