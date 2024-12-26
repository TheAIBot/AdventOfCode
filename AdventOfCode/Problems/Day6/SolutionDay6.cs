using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Problems.Day6
{
    public sealed class SolutionDay6 : IAdventProblem
    {
        public async Task ExecuteFirstPartAsync(CancellationToken cancellationToken)
        {
            Problem problem = await GetProblemInputAsync(cancellationToken);

            IEnumerable<GuardMovement> guardMovements = GetGuardMovement(problem.Map, problem.MapSize, problem.GuardStartingPosition, Direction.Up);
            MovementAnalysis movementAnalysis = AnalyzeGuardMovements(problem.MapSize, guardMovements);

            Console.WriteLine(movementAnalysis.UniqueTilesCoveredCount);
        }

        public async Task ExecuteSecondPartAsync(CancellationToken cancellationToken)
        {
            Problem problem = await GetProblemInputAsync(cancellationToken);

            TileType[][] mapForFindingLoops = CopyMap(problem.Map);


            HashSet<Point> loopObstablePoistions = [];
            foreach (GuardMovement guardMovement in GetGuardMovement(problem.Map, problem.MapSize, problem.GuardStartingPosition, Direction.Up))
            {
                Point obstableMoveVector = GetMoveVectorFromDirection(guardMovement.Direction);
                Point obstaclePosition = new Point(guardMovement.Position.X + obstableMoveVector.X, guardMovement.Position.Y + obstableMoveVector.Y);

                if (IsOutsideMap(obstaclePosition, problem.MapSize))
                {
                    continue;
                }

                if (problem.Map[obstaclePosition.Y][obstaclePosition.X] == TileType.Wall)
                {
                    continue;
                }

                if (obstaclePosition == problem.GuardStartingPosition)
                {
                    continue;
                }

                mapForFindingLoops[obstaclePosition.Y][obstaclePosition.X] = TileType.Wall;

                IEnumerable<GuardMovement> potentiallyLoopingGuardMovements = GetGuardMovement(mapForFindingLoops, problem.MapSize, guardMovement.Position, guardMovement.Direction);
                MovementAnalysis potentiallyLoopingMapAnalysis = AnalyzeGuardMovements(problem.MapSize, potentiallyLoopingGuardMovements);

                mapForFindingLoops[obstaclePosition.Y][obstaclePosition.X] = TileType.Air;

                if (potentiallyLoopingMapAnalysis.MovementEndReason != MovementEndReason.Loop)
                {
                    continue;
                }

                loopObstablePoistions.Add(obstaclePosition);
            }

            Console.WriteLine(loopObstablePoistions.Count);
        }

        private static async Task<Problem> GetProblemInputAsync(CancellationToken cancellationToken)
        {
            string[] textMap = await File.ReadAllLinesAsync("problems/day6/input.txt", cancellationToken);
            Point? guardPosition = null;
            for (int y = 0; y < textMap.Length; y++)
            {
                int guardIndex = textMap[y].IndexOf('^');
                if (guardIndex == -1)
                {
                    continue;
                }

                guardPosition = new Point(guardIndex, y);
            }

            Point mapSize = new Point(textMap[0].Length, textMap.Length);
            TileType[][] map = CreateTileMap(textMap);

            if (guardPosition == null)
            {
                throw new InvalidOperationException("Failed to find guard initial position.");
            }

            return new Problem(map, mapSize, guardPosition.Value);
        }

        public static TileType[][] CreateTileMap(string[] textMap)
        {
            TileType[][] map = new TileType[textMap.Length][];
            for (int y = 0; y < textMap.Length; y++)
            {
                map[y] = new TileType[textMap[0].Length];
                for (int x = 0; x < map[y].Length; x++)
                {
                    TileType tile = textMap[y][x] switch
                    {
                        '#' => TileType.Wall,
                        '.' => TileType.Air,
                        '^' => TileType.Air,
                        _ => throw new InvalidOperationException("Unexpected map character."),
                    };
                    map[y][x] = tile;
                }
            }

            return map;
        }

        public static IEnumerable<GuardMovement> GetGuardMovement(TileType[][] map, Point mapSize, Point guardStartPosition, Direction guardStartDirection)
        {
            Dictionary<Point, Direction> walkedPositionWithDirection = [];
            Point guardPosition = guardStartPosition;
            Direction guardDirection = guardStartDirection;
            while (true)
            {
                walkedPositionWithDirection.TryAdd(guardPosition, guardDirection);
                walkedPositionWithDirection[guardPosition] |= guardDirection;
                yield return new GuardMovement(guardPosition, guardDirection);

                Point moveVector = GetMoveVectorFromDirection(guardDirection);
                Point nextPosition = new Point(guardPosition.X + moveVector.X, guardPosition.Y + moveVector.Y);
                bool guardLeftMap = false;
                while (nextPosition.X < 0 || nextPosition.X > mapSize.X - 1 ||
                       nextPosition.Y < 0 || nextPosition.Y > mapSize.Y - 1 ||
                       map[nextPosition.Y][nextPosition.X] != TileType.Air)
                {
                    if (nextPosition.X < 0 || nextPosition.X > mapSize.X - 1 ||
                        nextPosition.Y < 0 || nextPosition.Y > mapSize.Y - 1)
                    {
                        guardLeftMap = true;
                        break;
                    }
                    guardDirection = TurnDirectionRight(guardDirection);
                    moveVector = GetMoveVectorFromDirection(guardDirection);
                    nextPosition = new Point(guardPosition.X + moveVector.X, guardPosition.Y + moveVector.Y);
                }

                if (guardLeftMap)
                {
                    break;
                }

                guardPosition = nextPosition;

                if (walkedPositionWithDirection.TryGetValue(guardPosition, out Direction direction) && direction.HasFlag(guardDirection))
                {
                    yield return new GuardMovement(guardPosition, guardDirection);
                    break;
                }
            }
        }

        private static Point GetMoveVectorFromDirection(Direction direction) => direction switch
        {
            Direction.Up => new Point(0, -1),
            Direction.Down => new Point(0, 1),
            Direction.Left => new Point(-1, 0),
            Direction.Right => new Point(1, 0),
            _ => throw new InvalidOperationException("Unknown direction.")
        };

        private static Direction TurnDirectionRight(Direction direction) => direction switch
        {
            Direction.Up => Direction.Right,
            Direction.Down => Direction.Left,
            Direction.Left => Direction.Up,
            Direction.Right => Direction.Down,
            _ => throw new InvalidOperationException("Unknown direction.")
        };

        private static bool IsOutsideMap(Point position, Point mapSize)
        {
            return position.X < 0 || position.X > mapSize.X - 1 ||
                   position.Y < 0 || position.Y > mapSize.Y - 1;
        }

        private static TileType[][] CopyMap(TileType[][] map)
        {
            var copy = new TileType[map.Length][];
            for (int y = 0; y < map.Length; y++)
            {
                copy[y] = new TileType[map[0].Length];
                Array.Copy(map[y], copy[y], map[y].Length);
            }

            return copy;
        }

        private static MovementAnalysis AnalyzeGuardMovements(Point mapSize, IEnumerable<GuardMovement> guardMovements)
        {
            Dictionary<Point, Direction> walkedPositionWithDirection = [];
            GuardMovement? lastGuardMovement = null;
            foreach (var guardMovement in guardMovements)
            {
                lastGuardMovement = guardMovement;

                walkedPositionWithDirection.TryAdd(guardMovement.Position, guardMovement.Direction);
                walkedPositionWithDirection[guardMovement.Position] |= guardMovement.Direction;
            }

            if (lastGuardMovement == null)
            {
                throw new InvalidOperationException("Unable to analyze a guards movements with no movements.");
            }

            Point moveVector = GetMoveVectorFromDirection(lastGuardMovement.Value.Direction);
            Point nextPosition = new Point(lastGuardMovement.Value.Position.X + moveVector.X, lastGuardMovement.Value.Position.Y + moveVector.Y);
            MovementEndReason movementEndReason = IsOutsideMap(nextPosition, mapSize) ? MovementEndReason.MovedOutOfMap : MovementEndReason.Loop;

            return new MovementAnalysis(walkedPositionWithDirection.Count, movementEndReason);
        }
    }

    public readonly record struct Point(int X, int Y);
    public readonly record struct Problem(TileType[][] Map, Point MapSize, Point GuardStartingPosition);
    public readonly record struct GuardMovement(Point Position, Direction Direction);
    public readonly record struct MovementAnalysis(int UniqueTilesCoveredCount, MovementEndReason MovementEndReason);

    [Flags]
    public enum Direction
    {
        Up = 0b0001,
        Down = 0b0010,
        Left = 0b0100,
        Right = 0b1000
    }

    public enum TileType : byte
    {
        Wall,
        Air,
    }

    public enum MovementEndReason
    {
        MovedOutOfMap,
        Loop
    }
}
