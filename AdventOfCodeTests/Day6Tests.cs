using AdventOfCode.Problems.Day6;

namespace AdventOfCodeTests;

public sealed class Day6Tests
{
    [Test]
    public async Task GetGuardMovement_With2x2MapWithNoWalls_ExpectCorrectMovements()
    {
        var map = SolutionDay6.CreateTileMap("""
            ..
            ..
            """.Split(Environment.NewLine));
        var mapSize = new Point(map[0].Length, map.Length);
        var guardStartPosition = new Point(0, 1);
        var guardStartDirection = Direction.Up;
        GuardMovement[] expectedMovement = [
            new GuardMovement(new Point(0, 1), Direction.Up),
            new GuardMovement(new Point(0, 0), Direction.Up)
        ];

        var actualMovement = SolutionDay6.GetGuardMovement(map, mapSize, guardStartPosition, guardStartDirection);

        await Assert.That(actualMovement).IsEquivalentTo(expectedMovement);
    }

    [Test]
    public async Task GetGuardMovement_With2x2MapWithSingleDirectionChange_ExpectCorrectMovements()
    {
        var map = SolutionDay6.CreateTileMap("""
            #.
            ..
            """.Split(Environment.NewLine));
        var mapSize = new Point(map[0].Length, map.Length);
        var guardStartPosition = new Point(0, 1);
        var guardStartDirection = Direction.Up;
        GuardMovement[] expectedMovement = [
            new GuardMovement(new Point(0, 1), Direction.Up),
            new GuardMovement(new Point(1, 1), Direction.Right)
        ];

        var actualMovement = SolutionDay6.GetGuardMovement(map, mapSize, guardStartPosition, guardStartDirection);

        await Assert.That(actualMovement).IsEquivalentTo(expectedMovement);
    }

    [Test]
    public async Task GetGuardMovement_With3x3MapWithTwoDirectionChanges_ExpectCorrectMovements()
    {
        var map = SolutionDay6.CreateTileMap("""
            #..
            ..#
            ...
            """.Split(Environment.NewLine));
        var mapSize = new Point(map[0].Length, map.Length);
        var guardStartPosition = new Point(0, 2);
        var guardStartDirection = Direction.Up;
        GuardMovement[] expectedMovement = [
            new GuardMovement(new Point(0, 2), Direction.Up),
            new GuardMovement(new Point(0, 1), Direction.Up),
            new GuardMovement(new Point(1, 1), Direction.Right),
            new GuardMovement(new Point(1, 2), Direction.Down)
        ];

        var actualMovement = SolutionDay6.GetGuardMovement(map, mapSize, guardStartPosition, guardStartDirection);

        await Assert.That(actualMovement).IsEquivalentTo(expectedMovement);
    }

    [Test]
    public async Task GetGuardMovement_With4x4MapWithLoop_ExpectCorrectMovements()
    {
        var map = SolutionDay6.CreateTileMap("""
            .#..
            ...#
            #...
            ..#.
            """.Split(Environment.NewLine));
        var mapSize = new Point(map[0].Length, map.Length);
        var guardStartPosition = new Point(1, 2);
        var guardStartDirection = Direction.Left;
        GuardMovement[] expectedMovement = [
            new GuardMovement(new Point(1, 2), Direction.Left),
            new GuardMovement(new Point(1, 1), Direction.Up),
            new GuardMovement(new Point(2, 1), Direction.Right),
            new GuardMovement(new Point(2, 2), Direction.Down),
            new GuardMovement(new Point(1, 2), Direction.Left)
        ];

        var actualMovement = SolutionDay6.GetGuardMovement(map, mapSize, guardStartPosition, guardStartDirection);

        await Assert.That(actualMovement).IsEquivalentTo(expectedMovement);
    }
}
