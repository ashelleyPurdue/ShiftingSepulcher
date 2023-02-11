using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace ShiftingSepulcher
{
    public class RandomWallsPopulator : Node, IRoomPopulator
    {
        public void Populate(DungeonTreeRoom treeRoom, Random rng)
        {
            // Randomly knock down walls until all cells are connected
            var graph = new WallsGraph();

            while (!graph.AreAllCellsConnected())
            {
                var wall = rng.PickFrom(graph.AllUndemolishedWalls());
                graph.DemolishWall(wall);
            }

            ApplyDemolishedWalls(graph);
        }

        private void ApplyDemolishedWalls(WallsGraph graph)
        {
            // Destroy all the demolished walls
            foreach (var wall in graph.AllDemolishedWalls())
            {
                GetRealWall(wall).QueueFree();
            }

            // Destroy pillars that have no adjacent undemolished walls
            foreach (var pillar in PillarId.All())
            {
                bool shouldDestroy = pillar
                    .AdjacentWalls()
                    .All(graph.IsWallDemolished);

                if (shouldDestroy)
                    GetRealPillar(pillar).QueueFree();
            }
        }

        private Node2D GetRealWall(WallId wall)
        {
            return GetNode<Node2D>($"%WallSectors/{wall.Sector}/{wall.Wall}");
        }

        private Node2D GetRealPillar(PillarId pillar)
        {
            return GetNode<Node2D>($"%Pillars/{pillar.Vert}{pillar.Hor}");
        }

        private struct WallId
        {
            public readonly CardinalDirection Sector;
            public readonly CardinalDirection Wall;

            public WallId(CardinalDirection sector, CardinalDirection wall)
            {
                if (sector == wall)
                {
                    throw new Exception($"Sector {sector} does not have a {wall} wall");
                }

                Sector = sector;
                Wall = wall;
            }

            public static IEnumerable<WallId> All()
            {
                foreach (var sector in CardinalDirectionUtils.All())
                {
                    foreach (var wall in CardinalDirectionUtils.All())
                    {
                        if (sector == wall)
                            continue;

                        yield return new WallId(sector, wall);
                    }
                }
            }
        }

        private struct PillarId
        {
            public readonly CardinalDirection Vert;
            public readonly CardinalDirection Hor;

            public PillarId(CardinalDirection vertical, CardinalDirection horizontal)
            {
                Vert = vertical;
                Hor = horizontal;
            }

            public static IEnumerable<PillarId> All() => new[]
            {
                new PillarId(CardinalDirection.North, CardinalDirection.West),
                new PillarId(CardinalDirection.North, CardinalDirection.East),
                new PillarId(CardinalDirection.South, CardinalDirection.West),
                new PillarId(CardinalDirection.South, CardinalDirection.East)
            };

            public IEnumerable<WallId> AdjacentWalls() => new[]
            {
                new WallId(Vert, Hor),
                new WallId(Vert, Vert.Opposite()),
                new WallId(Hor, Vert),
                new WallId(Hor, Hor.Opposite())
            };
        }

        private class WallsGraph
        {
            private const int Rows = 3;
            private const int Columns = 3;

            private HashSet<WallId> _demolishedWalls = new HashSet<WallId>();

            public IEnumerable<WallId> AllDemolishedWalls()
            {
                return _demolishedWalls;
            }

            public IEnumerable<WallId> AllUndemolishedWalls()
            {
                return WallId
                    .All()
                    .Where(w => !IsWallDemolished(w));
            }

            public void DemolishWall(WallId wall)
            {
                _demolishedWalls.Add(wall);
            }

            public bool AreAllCellsConnected()
            {
                var reachableCells = TraverseCells(new Vector2i(1, 1)).ToHashSet();

                for (int x = 0; x < Columns; x++)
                {
                    for (int y = 0; y < Rows; y++)
                    {
                        var cell = new Vector2i(x, y);
                        bool reachable = reachableCells.Contains(cell);

                        if (!reachable)
                            return false;
                    }
                }

                return true;
            }

            public bool IsWallDemolished(WallId wall)
            {
                return _demolishedWalls.Contains(wall);
            }

            private bool IsInBounds(Vector2i cell)
            {
                return
                    cell.x >= 0 &&
                    cell.x < Columns &&
                    cell.y >= 0 &&
                    cell.y < Rows;
            }

            private IEnumerable<Vector2i> Neighbors(Vector2i cell)
            {
                foreach (var dir in CardinalDirectionUtils.All())
                {
                    var neighbor = cell.Adjacent(dir);

                    if (IsInBounds(neighbor))
                        yield return neighbor;
                }
            }

            private bool AreNeighborsImmediatelyConnected(
                Vector2i cellA,
                Vector2i cellB
            )
            {
                AssertValidNeighbors(cellA, cellB);

                var wall = GetConnectingWall(cellA, cellB);
                return IsWallDemolished(wall);
            }

            private IEnumerable<Vector2i> TraverseCells(Vector2i startCell)
            {
                // Breadth first search.
                // This could also have been a depth first search, but I'm just
                // not in the mood for recursion today.
                var previouslyQueuedCells = new HashSet<Vector2i>();
                var visitQueue = new Queue<Vector2i>();

                visitQueue.Enqueue(startCell);
                previouslyQueuedCells.Add(startCell);

                while (visitQueue.Count > 0)
                {
                    var cell = visitQueue.Dequeue();
                    yield return cell;

                    var neighborsToQueue = Neighbors(cell)
                        .Where(n => AreNeighborsImmediatelyConnected(cell, n))
                        .Where(n => !previouslyQueuedCells.Contains(n));

                    foreach (var neighbor in neighborsToQueue)
                    {
                        visitQueue.Enqueue(neighbor);
                        previouslyQueuedCells.Add(neighbor);
                    }
                }
            }

            private WallId GetConnectingWall(Vector2i cellA, Vector2i cellB)
            {
                AssertValidNeighbors(cellA, cellB);

                var dirAToB = cellA.AdjacentDirection(cellB);

                var ca = (cellA.x, cellA.y);

                // Smack in the middle
                if (ca == (1, 1))
                {
                    return new WallId(dirAToB, dirAToB.Opposite());
                }

                // In the middle of each sector
                if (ca == (1, 2))
                {
                    return new WallId(CardinalDirection.North, dirAToB);
                }
                if (ca == (2, 1))
                {
                    return new WallId(CardinalDirection.East, dirAToB);
                }
                if (ca == (1, 0))
                {
                    return new WallId(CardinalDirection.South, dirAToB);
                }
                if (ca == (0, 1))
                {
                    return new WallId(CardinalDirection.West, dirAToB);
                }

                // Corners
                if (ca == (0, 2))
                {
                    switch (dirAToB)
                    {
                        case CardinalDirection.East: return new WallId(CardinalDirection.North, CardinalDirection.West);
                        case CardinalDirection.South: return new WallId(CardinalDirection.West, CardinalDirection.North);
                        default: throw Invalid();
                    }
                }
                if (ca == (2, 2))
                {
                    switch (dirAToB)
                    {
                        case CardinalDirection.West: return new WallId(CardinalDirection.North, CardinalDirection.East);
                        case CardinalDirection.South: return new WallId(CardinalDirection.East, CardinalDirection.North);
                        default: throw Invalid();
                    }
                }
                if (ca == (2, 0))
                {
                    switch (dirAToB)
                    {
                        case CardinalDirection.North: return new WallId(CardinalDirection.East, CardinalDirection.South);
                        case CardinalDirection.West: return new WallId(CardinalDirection.South, CardinalDirection.East);
                        default: throw Invalid();
                    }
                }
                if (ca == (0, 0))
                {
                    switch (dirAToB)
                    {
                        case CardinalDirection.North: return new WallId(CardinalDirection.West, CardinalDirection.South);
                        case CardinalDirection.East: return new WallId(CardinalDirection.South, CardinalDirection.West);
                        default: throw Invalid();
                    }
                }

                throw Invalid();

                Exception Invalid()
                {
                    return new Exception($"Branch should not be possible.  {ca}, {dirAToB}");
                }
            }

            private void AssertValidNeighbors(Vector2i cellA, Vector2i cellB)
            {
                if (!cellA.IsAdjacentTo(cellB))
                    throw new Exception($"{cellA} is not a neighbor to {cellB}");

                if (!IsInBounds(cellA))
                    throw new Exception($"{cellA} is out of bounds");

                if (!IsInBounds(cellB))
                    throw new Exception($"{cellB} is out of bounds");
            }
        }
    }
}
