namespace Deanery.Data.Constants;

public static class WorkTypes
{
    public const string Test = "Контрольная";
    public const string Exam = "Экзамен";
    public const string Coursework = "Курсовая";
    public const string Lab = "Лабораторная";

    public static readonly string[] All = [Test, Exam, Coursework, Lab];
}
