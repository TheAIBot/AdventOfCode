using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode.Problems.Day1;

internal sealed partial class SolutionDay1 : IAdventProblem
{
    public async Task ExecuteFirstPartAsync(CancellationToken cancellationToken)
    {
        (List<int> left, List<int> right) = await ReadInputAsync(cancellationToken);

        int differenceSum = left.Order()
                                .Zip(right.Order())
                                .Select(x => Math.Abs(x.First - x.Second))
                                .Sum();
        
        Console.WriteLine(differenceSum);
    }

    public async Task ExecuteSecondPartAsync(CancellationToken cancellationToken)
    {
        (List<int> left, List<int> right) = await ReadInputAsync(cancellationToken);
        Dictionary<int, int> leftNumberToRightCount = left.Distinct().ToDictionary(x => x, _ => 0);
        foreach (var rightNumber in right)
        {
            ref int rightCount = ref CollectionsMarshal.GetValueRefOrNullRef(leftNumberToRightCount, rightNumber);
            if (Unsafe.IsNullRef(ref rightCount))
            {
                continue;
            }

            rightCount++;
        }

        int similarityScore = leftNumberToRightCount.Select(x => x.Key * x.Value).Sum();
        Console.WriteLine(similarityScore);
    }

    private async Task<(List<int> left, List<int> right)> ReadInputAsync(CancellationToken cancellationToken)
    {
        List<int> left = [];
        List<int> right = [];
        await foreach (var line in File.ReadLinesAsync("problems/day1/input.txt", cancellationToken))
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            int spaceIndex = line.IndexOf(' ');
            left.Add(int.Parse(line.AsSpan(0, spaceIndex), CultureInfo.InvariantCulture));
            right.Add(int.Parse(line.AsSpan(spaceIndex), CultureInfo.InvariantCulture));
        }

        return (left, right);
    }
}
