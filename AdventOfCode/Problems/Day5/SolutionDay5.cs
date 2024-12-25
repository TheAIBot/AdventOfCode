using AdventOfCode.Problems.Day2;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Problems.Day5
{
    internal sealed class SolutionDay5 : IAdventProblem
    {
        public async Task ExecuteFirstPartAsync(CancellationToken cancellationToken)
        {
            ManualWithPageRules manualWithPageRules = await GetPageRulesAndManualsAsync(cancellationToken);
            Dictionary<Page, HashSet<Page>> pageToPagesThatMustNotBeSeenYet = CreatePageValidationRules(manualWithPageRules);

            int validManualsMiddlePageNumberSum = 0;
            foreach (Manual manual in manualWithPageRules.Manuals)
            {
                if (!AnalyseManual(pageToPagesThatMustNotBeSeenYet, manual).IsManualValid)
                {
                    continue;
                }

                validManualsMiddlePageNumberSum += manual.Pages[manual.Pages.Count / 2].PageNumber;
            }

            Console.WriteLine(validManualsMiddlePageNumberSum);
        }

        public async Task ExecuteSecondPartAsync(CancellationToken cancellationToken)
        {
            ManualWithPageRules manualWithPageRules = await GetPageRulesAndManualsAsync(cancellationToken);
            Dictionary<Page, HashSet<Page>> pageToPagesThatMustNotBeSeenYet = CreatePageValidationRules(manualWithPageRules);

            int validManualsMiddlePageNumberSum = 0;
            foreach (Manual manual in manualWithPageRules.Manuals)
            {
                ManualAnalysis manualAnalysis = AnalyseManual(pageToPagesThatMustNotBeSeenYet, manual);
                if (manualAnalysis.IsManualValid)
                {
                    continue;
                }

                Manual fixedManual = manual;
                if (!manualAnalysis.IsPageInValidLocation[manual.Pages.Count / 2])
                {
                    fixedManual = FixManual(manual, manualAnalysis, pageToPagesThatMustNotBeSeenYet);
                }

                validManualsMiddlePageNumberSum += fixedManual.Pages[manual.Pages.Count / 2].PageNumber;
            }

            Console.WriteLine(validManualsMiddlePageNumberSum);
        }

        private static async Task<ManualWithPageRules> GetPageRulesAndManualsAsync(CancellationToken cancellationToken)
        {
            List<PageRule> pageRules = [];
            List<Manual> manuals = [];
            bool isWorkingOnPageRules = true;
            await foreach (var line in File.ReadLinesAsync("problems/day5/input.txt", cancellationToken))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    isWorkingOnPageRules = false;
                    continue;
                }

                if (isWorkingOnPageRules)
                {
                    int separatorIndex = line.IndexOf('|');
                    int firstPage = int.Parse(line.AsSpan(0, separatorIndex), CultureInfo.InvariantCulture);
                    int secondPage = int.Parse(line.AsSpan(separatorIndex + 1), CultureInfo.InvariantCulture);
                    pageRules.Add(new PageRule(new Page(firstPage), new Page(secondPage)));
                }
                else
                {
                    List<Page> pages = [];
                    foreach (Range pageRange in line.AsSpan().Split(','))
                    {
                        pages.Add(new Page(int.Parse(line.AsSpan(pageRange), CultureInfo.InvariantCulture)));
                    }

                    manuals.Add(new Manual(pages));
                }
            }

            return new ManualWithPageRules(pageRules, manuals);
        }

        private static Dictionary<Page, HashSet<Page>> CreatePageValidationRules(ManualWithPageRules manualWithPageRules)
        {
            Dictionary<Page, HashSet<Page>> pageToPagesThatMustNotBeSeenYet = [];
            foreach (PageRule pageRule in manualWithPageRules.PageRules)
            {
                if (!pageToPagesThatMustNotBeSeenYet.TryGetValue(pageRule.PageBefore, out HashSet<Page>? pagesThatMustNotBeSeenYet))
                {
                    pagesThatMustNotBeSeenYet = [];
                    pageToPagesThatMustNotBeSeenYet.Add(pageRule.PageBefore, pagesThatMustNotBeSeenYet);
                }

                pagesThatMustNotBeSeenYet.Add(pageRule.PageAfter);
            }

            return pageToPagesThatMustNotBeSeenYet;
        }

        private static ManualAnalysis AnalyseManual(Dictionary<Page, HashSet<Page>> pageToPagesThatMustNotBeSeenYet, Manual manual)
        {
            bool isManualValid = true;
            bool[] isPageInValidLocation = new bool[manual.Pages.Count];
            Array.Fill(isPageInValidLocation, true);

            for (int pageIndex = 0; pageIndex < manual.Pages.Count; pageIndex++)
            {
                Page page = manual.Pages[pageIndex];

                if (!pageToPagesThatMustNotBeSeenYet.TryGetValue(page, out HashSet<Page>? pagesThatMustNotBeSeenYet))
                {
                    continue;
                }

                bool isPageLocationValid = true;
                for (int seenPagesIndex = 0; seenPagesIndex < pageIndex; seenPagesIndex++)
                {
                    Page seenPage = manual.Pages[seenPagesIndex];
                    if (pagesThatMustNotBeSeenYet.Contains(seenPage))
                    {
                        isPageInValidLocation[seenPagesIndex] = false;
                        isPageLocationValid = false;
                    }
                }

                if (!isPageLocationValid)
                {
                    isPageInValidLocation[pageIndex] = false;
                    isManualValid = false;
                }
            }

            return new ManualAnalysis(isManualValid, isPageInValidLocation);
        }

        private static Manual FixManual(Manual manual, ManualAnalysis manualAnalysis, Dictionary<Page, HashSet<Page>> pageToPagesThatMustNotBeSeenYet)
        {
            Page[] fixedPages = manual.Pages.ToArray();

            HashSet<Page> invalidPlacedPages = [];
            List<int> invalidPlacedPageIndexes = [];
            for (int i = 0; i < manual.Pages.Count; i++)
            {
                if (manualAnalysis.IsPageInValidLocation[i])
                {
                    continue;
                }

                invalidPlacedPages.Add(manual.Pages[i]);
                invalidPlacedPageIndexes.Add(i);
            }

            for (int i = invalidPlacedPageIndexes.Count - 1; i >= 0; i--)
            {
                Page pageThatCanBePlaced = invalidPlacedPages.First(x => pageToPagesThatMustNotBeSeenYet[x].Intersect(invalidPlacedPages).Count() == 0);
                invalidPlacedPages.Remove(pageThatCanBePlaced);

                fixedPages[invalidPlacedPageIndexes[i]] = pageThatCanBePlaced;
            }

            return new Manual(fixedPages);
        }

        private readonly record struct Page(int PageNumber);
        private readonly record struct PageRule(Page PageBefore, Page PageAfter);
        private readonly record struct Manual(IReadOnlyList<Page> Pages);
        private readonly record struct ManualWithPageRules(IReadOnlyList<PageRule> PageRules, IReadOnlyList<Manual> Manuals);
        private readonly record struct ManualAnalysis(bool IsManualValid, bool[] IsPageInValidLocation);
    }
}
