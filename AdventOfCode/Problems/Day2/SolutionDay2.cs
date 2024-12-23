using System.Globalization;
using System.Runtime.CompilerServices;

namespace AdventOfCode.Problems.Day2;

internal sealed class SolutionDay2 : IAdventProblem
{
    public async Task ExecuteFirstPartAsync(CancellationToken cancellationToken)
    {
        int safeReportCount = 0;
        await foreach (var report in GetReportsAsync(cancellationToken))
        {
            IEnumerator<int> levels = report.Levels.GetEnumerator();
            levels.MoveNext();

            int previousLevel = levels.Current;
            bool? isIncreasing = null;
            bool reportIsSafe = true;
            while (levels.MoveNext())
            {
                int currentLevel = levels.Current;
                if (isIncreasing == null)
                {
                    isIncreasing = currentLevel > previousLevel;
                }
                else if (isIncreasing == true && currentLevel < previousLevel)
                {
                    reportIsSafe = false;
                    break;
                }
                else if (isIncreasing == false && currentLevel > previousLevel)
                {
                    reportIsSafe = false;
                    break;
                }

                int change = Math.Abs(currentLevel - previousLevel);
                if (change < 1 || change > 3)
                {
                    reportIsSafe = false;
                    break;
                }

                previousLevel = currentLevel;
            }

            if (reportIsSafe)
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
            //int[] levelPartOfRuleViolationCount = report.Levels.Count;
            IEnumerator<int> levels = report.Levels.GetEnumerator();
            levels.MoveNext();

            int previousLevel = levels.Current;
            bool? isIncreasing = null;
            bool reportIsSafe = true;
            while (levels.MoveNext())
            {
                int currentLevel = levels.Current;
                if (isIncreasing == null)
                {
                    isIncreasing = currentLevel > previousLevel;
                }
                else if (isIncreasing == true && currentLevel < previousLevel)
                {
                    reportIsSafe = false;
                    break;
                }
                else if (isIncreasing == false && currentLevel > previousLevel)
                {
                    reportIsSafe = false;
                    break;
                }

                int change = Math.Abs(currentLevel - previousLevel);
                if (change < 1 || change > 3)
                {
                    reportIsSafe = false;
                    break;
                }

                previousLevel = currentLevel;
            }

            if (reportIsSafe)
            {
                safeReportCount++;
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

    private readonly record struct Report(List<int> Levels);
}
