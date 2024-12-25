using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Problems.Day4
{
    internal sealed class SolutionDay4 : IAdventProblem
    {
        public async Task ExecuteFirstPartAsync(CancellationToken cancellationToken)
        {
            string[] lines = await File.ReadAllLinesAsync("problems/day4/input.txt", cancellationToken);
            if (lines.Select(x => x.Length).Distinct().Count() != 1)
            {
                throw new InvalidOperationException("All lines must ohave the same length");
            }

            ReadOnlySpan<char> xmas = "XMAS";
            int xmasCount = 0;
            for (int y = 0; y < lines.Length; y++)
            {
                for (int x = 0; x < lines[y].Length; x++)
                {
                    xmasCount += HasWordInDirection(x, y, 1, 0, xmas, lines) ? 1 : 0;
                    xmasCount += HasWordInDirection(x, y, 1, 1, xmas, lines) ? 1 : 0;
                    xmasCount += HasWordInDirection(x, y, 0, 1, xmas, lines) ? 1 : 0;
                    xmasCount += HasWordInDirection(x, y, -1, 1, xmas, lines) ? 1 : 0;
                    xmasCount += HasWordInDirection(x, y, -1, 0, xmas, lines) ? 1 : 0;
                    xmasCount += HasWordInDirection(x, y, -1, -1, xmas, lines) ? 1 : 0;
                    xmasCount += HasWordInDirection(x, y, 0, -1, xmas, lines) ? 1 : 0;
                    xmasCount += HasWordInDirection(x, y, 1, -1, xmas, lines) ? 1 : 0;
                }
            }

            Console.WriteLine(xmasCount);
        }

        public async Task ExecuteSecondPartAsync(CancellationToken cancellationToken)
        {
            string[] lines = await File.ReadAllLinesAsync("problems/day4/input.txt", cancellationToken);
            if (lines.Select(x => x.Length).Distinct().Count() != 1)
            {
                throw new InvalidOperationException("All lines must ohave the same length");
            }

            ReadOnlySpan<char> mas = "MAS";
            int masCount = 0;
            for (int y = 0; y < lines.Length; y++)
            {
                for (int x = 0; x < lines[y].Length; x++)
                {
                    masCount += (HasWordInDirection(x - 1, y - 1, 1, 1, mas, lines) || 
                                 HasWordInDirection(x + 1, y + 1, -1, -1, mas, lines)) &&
                                (HasWordInDirection(x - 1, y + 1, 1, -1, mas, lines) || 
                                 HasWordInDirection(x + 1, y - 1, -1, 1, mas, lines)) ? 1 : 0;
                }
            }

            Console.WriteLine(masCount);
        }

        private static bool HasWordInDirection(int startX, int startY, int xDelta, int yDelta, ReadOnlySpan<char> word, string[] textMatrix)
        {
            if (!IsWithinMatrix(startX, startY, textMatrix))
            {
                return false;
            }

            for (int i = 0; i < word.Length; i++)
            {
                int x = startX + (i * xDelta);
                int y = startY + (i * yDelta);
                if (!IsWithinMatrix(x, y, textMatrix))
                {
                    return false;
                }

                if (textMatrix[y][x] != word[i])
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsWithinMatrix(int x, int y, string[] textMatrix)
        {
            if (x < 0 || x > textMatrix[0].Length - 1)
            {
                return false;
            }

            if (y < 0 || y > textMatrix.Length - 1)
            {
                return false;
            }

            return true;
        }
    }
}
