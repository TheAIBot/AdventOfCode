using System.Globalization;
using System.Runtime.CompilerServices;

namespace AdventOfCode.Problems.Day2;

public sealed class SolutionDay2 : IAdventProblem
{
    public async Task ExecuteFirstPartAsync(CancellationToken cancellationToken)
    {
        int safeReportCount = 0;
        await foreach (var report in GetReportsAsync(cancellationToken))
        {
            if (IsReportViolatingAnyRules(report))
            {
                safeReportCount++;
            }
        }

        Console.WriteLine(safeReportCount);
    }

    public async Task ExecuteSecondPartAsync(CancellationToken cancellationToken)
    {
        int safeReportCount = 0;
        await foreach (var report in GetReportsAsync(cancellationToken))
        {
            AnalyzedReport reportAnalysis = AnalyseReport(report);
            if (reportAnalysis.LevelDirection == LevelDirection.Unknown)
            {
                continue;
            }

            if (Array.TrueForAll(reportAnalysis.LevelsViolationCount, x => x == 0))
            {
                safeReportCount++;
                continue;
            }

            int levelsWithTwoRulePairViolationsCount = reportAnalysis.LevelsViolationCount.Count(x => x == 2);
            if (levelsWithTwoRulePairViolationsCount > 1)
            {
                continue;
            }

            int levelsWithOneRulePairViolationsCount = reportAnalysis.LevelsViolationCount.Count(x => x == 1);
            if (levelsWithTwoRulePairViolationsCount > 2)
            {
                continue;
            }

            if (levelsWithTwoRulePairViolationsCount > 0)
            {
                int firstTwoRuleViolationsLevelIndex = Array.IndexOf(reportAnalysis.LevelsViolationCount, 2);
                Report reportWithViolationLevelRemoved = new Report(report.Levels.ToList());
                //reportWithViolationLevelRemoved.Levels.RemoveAt(firstTwoRuleViolationsLevelIndex);
                if (IsReportViolatingAnyRules(reportWithViolationLevelRemoved))
                {
                    safeReportCount++;
                    continue;
                }
            }
            else
            {
                int firstOneRuleViolationLevelIndex = Array.IndexOf(reportAnalysis.LevelsViolationCount, 1);
                Report reportWithFirstViolationLevelRemoved = new Report(report.Levels.ToList());
                //reportWithFirstViolationLevelRemoved.Levels.RemoveAt(firstOneRuleViolationLevelIndex);
                if (IsReportViolatingAnyRules(reportWithFirstViolationLevelRemoved))
                {
                    safeReportCount++;
                    continue;
                }

                int secondOneRuleViolationLevelIndex = Array.IndexOf(reportAnalysis.LevelsViolationCount, 1, firstOneRuleViolationLevelIndex + 1);
                Report reportWithSecondViolationLevelRemoved = new Report(report.Levels.ToList());
                //reportWithSecondViolationLevelRemoved.Levels.RemoveAt(secondOneRuleViolationLevelIndex);
                if (IsReportViolatingAnyRules(reportWithSecondViolationLevelRemoved))
                {
                    safeReportCount++;
                    continue;
                }
            }
        }

        Console.WriteLine(safeReportCount);
    }

    private static async IAsyncEnumerable<Report> GetReportsAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        static List<int> ReadLevels(string report)
        {
            List<int> levels = [];
            foreach (Range level in report.AsSpan().Split(' '))
            {
                levels.Add(int.Parse(report.AsSpan(level), CultureInfo.InvariantCulture));
            }

            return levels;
        }

        await foreach (var report in File.ReadLinesAsync("problems/day2/input.txt", cancellationToken))
        {
            yield return new Report(ReadLevels(report));
        }
    }

    private static bool IsReportViolatingAnyRules(Report report)
    {
        AnalyzedReport reportAnalysis = AnalyseReport(report);
        return !reportAnalysis.ViolatesAnyRules;
    }

    public static AnalyzedReport AnalyseReport(Report report)
    {
        IReadOnlyList<int> levels = report.Levels;
        if (levels.Count < 2)
        {
            throw new InvalidOperationException("A report must consist of at least two levels to check whether it is safe or not.");
        }

        int[] levelPartOfRuleViolationCount = new int[report.Levels.Count];
        LevelDirection levelDirection = DetermineLevelDirection(levels);
        if (levelDirection == LevelDirection.Unknown)
        {
            return new AnalyzedReport(report, LevelDirection.Unknown, levelPartOfRuleViolationCount);
        }

        for (int levelIndex = 1; levelIndex < levels.Count; levelIndex++)
        {
            int previousLevel = levels[levelIndex - 1];
            int currentLevel = levels[levelIndex];

            if (IsViolatingAnyRules(previousLevel, currentLevel, levelDirection))
            {
                levelPartOfRuleViolationCount[levelIndex - 1]++;
                levelPartOfRuleViolationCount[levelIndex]++;
            }
        }

        return new AnalyzedReport(report, levelDirection, levelPartOfRuleViolationCount);
    }

    private static LevelDirection DetermineLevelDirection(IReadOnlyList<int> levels)
    {
        int levelIncreasingCount = 0;
        int levelDecreasingCount = 0;
        for (int levelIndex = 1; levelIndex < levels.Count; levelIndex++)
        {
            int previousLevel = levels[levelIndex - 1];
            int currentLevel = levels[levelIndex];

            if (previousLevel < currentLevel)
            {
                levelIncreasingCount++;
            }
            else if (previousLevel > currentLevel)
            {
                levelDecreasingCount++;
            }
        }

        if ((levelIncreasingCount == 0 &&
             levelDecreasingCount == 0) ||
            (levelIncreasingCount > 1 &&
             levelDecreasingCount > 1))
        {
            return LevelDirection.Unknown;
        }

        return levelIncreasingCount > levelDecreasingCount ? LevelDirection.Up : LevelDirection.Down;
    }

    private static bool IsViolatingAnyRules(int previousLevel, int currentLevel, LevelDirection levelDirection)
    {
        if (levelDirection == LevelDirection.Unknown)
        {
            return true;
        }

        int change = Math.Abs(currentLevel - previousLevel);
        if (change < 1 || change > 3)
        {
            return true;
        }


        if (levelDirection == LevelDirection.Up && currentLevel < previousLevel)
        {
            return true;
        }
        else if (levelDirection == LevelDirection.Down && currentLevel > previousLevel)
        {
            return true;
        }

        return false;
    }
}

public readonly record struct Report(IReadOnlyList<int> Levels);

public sealed record AnalyzedReport(Report Report, LevelDirection LevelDirection, int[] LevelsViolationCount)
{
    public bool ViolatesAnyRules => LevelDirection == LevelDirection.Unknown ||
                                    LevelsViolationCount.Any(x => x != 0);
}
public enum LevelDirection
{
    Up,
    Down,
    Unknown
}
