using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode.Problems.Day3
{
    internal sealed partial class SolutionDay3 : IAdventProblem
    {
        private const string _mulPrefix = "mul(";
        private const string _doCommand = "do()";
        private const string _doNotCommand = "don't()";

        public async Task ExecuteFirstPartAsync(CancellationToken cancellationToken)
        {
            long sum = 0;
            string memory = await File.ReadAllTextAsync("problems/day3/input.txt", cancellationToken);
            foreach (ValueMatch patternMatch in MultiplyPattern().EnumerateMatches(memory))
            {
                ReadOnlySpan<char> match = memory.AsSpan(patternMatch.Index, patternMatch.Length);

                int commaIndex = match.IndexOf(',');
                long firstNumber = long.Parse(match.Slice(_mulPrefix.Length, commaIndex - _mulPrefix.Length), CultureInfo.InvariantCulture);
                long secondNumber = long.Parse(match.Slice(commaIndex + 1).TrimEnd(')'), CultureInfo.InvariantCulture);

                sum += firstNumber * secondNumber;
            }

            Console.WriteLine(sum);
        }

        public async Task ExecuteSecondPartAsync(CancellationToken cancellationToken)
        {
            long sum = 0;
            bool isEnabled = true;
            string memory = await File.ReadAllTextAsync("problems/day3/input.txt", cancellationToken);
            foreach (ValueMatch patternMatch in AllCommandsPattern().EnumerateMatches(memory))
            {
                ReadOnlySpan<char> match = memory.AsSpan(patternMatch.Index, patternMatch.Length);
                if (match.SequenceEqual(_doCommand))
                {
                    isEnabled = true;
                    continue;
                }

                if (match.SequenceEqual(_doNotCommand))
                {
                    isEnabled = false;
                    continue;
                }

                if (!isEnabled)
                {
                    continue;
                }

                int commaIndex = match.IndexOf(',');
                long firstNumber = long.Parse(match.Slice(_mulPrefix.Length, commaIndex - _mulPrefix.Length), CultureInfo.InvariantCulture);
                long secondNumber = long.Parse(match.Slice(commaIndex + 1).TrimEnd(')'), CultureInfo.InvariantCulture);

                sum += firstNumber * secondNumber;
            }

            Console.WriteLine(sum);
        }

        [GeneratedRegex(@"mul\(\d+,\d+\)")]
        private static partial Regex MultiplyPattern();

        [GeneratedRegex(@"(do\(\)|don't\(\)|mul\(\d+,\d+\))")]
        private static partial Regex AllCommandsPattern();
    }
}
