namespace Deanery.Data.Constants;

public static class StudentStatuses
{
    public const string Active = "Обучается";
    public const string Expelled = "Отчислен";
    public const string AcademicLeave = "Академ";

    public static readonly string[] All = [Active, Expelled, AcademicLeave];
}
