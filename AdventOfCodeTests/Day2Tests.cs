using AdventOfCode.Problems.Day2;

namespace AdventOfCodeTests;

public sealed class Day2Tests
{
    [Test]
    [Arguments(new int[] { 1, 1 })]
    [Arguments(new int[] { 1, 2, 1 })]
    [Arguments(new int[] { 2, 1, 2 })]
    [Arguments(new int[] { 1, 5 })]
    [Arguments(new int[] { 5, 1 })]
    [Arguments(new int[] { 1, 2, 4, 8 })]
    [Arguments(new int[] { 8, 4, 2, 1 })]
    [Arguments(new int[] { 1, 2, 3, 2, 1 })]
    public async Task AnalyseReport_WithSingleRuleViolation_ExpectIsViolatingRule(int[] levels)
    {
        Report report = new Report(levels);

        AnalyzedReport reportAnalysis = SolutionDay2.AnalyseReport(report);

        await Assert.That(reportAnalysis.ViolatesAnyRules).IsTrue();
    }

    [Test]
    [Arguments(new int[] { 1, 1, 1, 1, 1, 1, 1, 1 })]
    [Arguments(new int[] { 1, 2, 3, 2, 1, 2, 3 })]
    [Arguments(new int[] { 1, 8, 16 })]
    [Arguments(new int[] { 16, 8, 1 })]
    [Arguments(new int[] { 1, 2, 3, 4, 3, 9 })]
    public async Task AnalyseReport_WithMultipleRuleViolations_ExpectIsViolatingRule(int[] levels)
    {
        Report report = new Report(levels);

        AnalyzedReport reportAnalysis = SolutionDay2.AnalyseReport(report);

        await Assert.That(reportAnalysis.ViolatesAnyRules).IsTrue();
    }

    [Test]
    [Arguments(new int[] { 1, 2 })]
    [Arguments(new int[] { 2, 1 })]
    [Arguments(new int[] { 1, 4 })]
    [Arguments(new int[] { 4, 1 })]
    [Arguments(new int[] { 1, 2, 3, 4, 5 })]
    [Arguments(new int[] { 5, 4, 3, 2, 1 })]
    [Arguments(new int[] { 1, 2, 4, 7 })]
    [Arguments(new int[] { 7, 4, 2, 1 })]
    public async Task AnalyseReport_WithSafeReport_ExpectDoesNotViolateAnyRules(int[] levels)
    {
        Report report = new Report(levels);

        AnalyzedReport reportAnalysis = SolutionDay2.AnalyseReport(report);

        await Assert.That(reportAnalysis.ViolatesAnyRules).IsFalse();
    }
}
