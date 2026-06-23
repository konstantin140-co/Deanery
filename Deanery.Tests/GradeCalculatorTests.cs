using Deanery.Data.Helpers;

namespace Deanery.Tests;

[TestFixture]
public class GradeCalculatorTests
{
    [Test]
    public void CalculateAverage_Empty_ReturnsZero()
    {
        Assert.That(GradeCalculator.CalculateAverage([]), Is.EqualTo(0m));
    }

    [Test]
    public void CalculateAverage_SingleScore_ReturnsScore()
    {
        Assert.That(GradeCalculator.CalculateAverage([5]), Is.EqualTo(5m));
    }

    [Test]
    public void CalculateAverage_MultipleScores_ReturnsRoundedAverage()
    {
        Assert.That(GradeCalculator.CalculateAverage([4, 5, 3]), Is.EqualTo(4m));
    }

    [TestCase(4.5, "Отлично")]
    [TestCase(4.0, "Хорошо")]
    [TestCase(3.0, "Удовлетворительно")]
    [TestCase(2.4, "Неудовлетворительно")]
    [TestCase(0, "Нет оценок")]
    public void GetPerformanceLevel_ReturnsExpected(double avg, string expected)
    {
        Assert.That(GradeCalculator.GetPerformanceLevel((decimal)avg), Is.EqualTo(expected));
    }

    [Test]
    public void HasFailingGrades_WithTwo_ReturnsTrue()
    {
        Assert.That(GradeCalculator.HasFailingGrades([3, 2, 4]), Is.True);
    }

    [Test]
    public void HasFailingGrades_WithoutTwo_ReturnsFalse()
    {
        Assert.That(GradeCalculator.HasFailingGrades([3, 4, 5]), Is.False);
    }

    [Test]
    public void CalculateAverage_FiveScores_Correct()
    {
        Assert.That(GradeCalculator.CalculateAverage([5, 5, 4, 4, 3]), Is.EqualTo(4.2m));
    }

    [Test]
    public void GetPerformanceLevel_ExcellentBoundary()
    {
        Assert.That(GradeCalculator.GetPerformanceLevel(4.49m), Is.EqualTo("Хорошо"));
        Assert.That(GradeCalculator.GetPerformanceLevel(4.5m), Is.EqualTo("Отлично"));
    }
}
