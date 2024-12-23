namespace AdventOfCode;

internal interface IAdventProblem
{
    Task ExecuteFirstPartAsync(CancellationToken cancellationToken);
    Task ExecuteSecondPartAsync(CancellationToken cancellationToken);
}