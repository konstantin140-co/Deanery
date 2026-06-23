namespace Deanery.Data.Helpers;

public static class GradeCalculator
{
    public static decimal CalculateAverage(IEnumerable<int> scores)
    {
        var list = scores.ToList();
        if (list.Count == 0)
            return 0m;

        return Math.Round((decimal)list.Average(), 2);
    }

    public static string GetPerformanceLevel(decimal average) => average switch
    {
        >= 4.5m => "Отлично",
        >= 3.5m => "Хорошо",
        >= 2.5m => "Удовлетворительно",
        > 0m => "Неудовлетворительно",
        _ => "Нет оценок"
    };

    public static bool HasFailingGrades(IEnumerable<int> scores)
        => scores.Any(s => s == 2);
}
