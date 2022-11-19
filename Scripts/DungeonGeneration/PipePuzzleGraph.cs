using System;
using System.Collections.Generic;
using System.Linq;

namespace RandomDungeons
{
    public class PipePuzzleGraph
    {
        public readonly int Width;
        public readonly int Height;

        private readonly Dictionary<Vector2i, Cell> _cells = new Dictionary<Vector2i, Cell>();

        public static PipePuzzleGraph Generate(
            Random rng,
            int width = 10,
            int height = 10,
            int minGrowths = 15,
            int numSinks = 3
        )
        {
            var puzzle = new PipePuzzleGraph(width, height);

            // Choose a random position for the source
            var source = new Vector2i(
                rng.Next(width),
                rng.Next(height)
            );
            puzzle.GetCell(source).Type = CellType.Source;

            // Choose a bunch of cells to "grow" another cell off of
            for (int i = 0; i < minGrowths || AllNonSinkLeaves().Count() < numSinks; i++)
            {
                var growthCandidates = GrowthCandidates().ToArray();

                if (growthCandidates.Length == 0)
                    break;

                var growth = rng.PickFrom(growthCandidates);

                var parentPos = growth.cellPos;
                var parentCell = puzzle.GetCell(parentPos);

                var childPos = parentPos + growth.dir.ToVector2i();
                var childCell = puzzle.GetCell(childPos);
                childCell.Type = CellType.Pipe;

                parentCell.SetDirectionOpen(growth.dir, true);
                childCell.SetDirectionOpen(growth.dir.Opposite(), true);
            }

            // Mark all "leaf" cells as sinks
            var leafCells = puzzle._cells
                .Select(kvp => kvp.Value)
                .Where(c => c.NumOpenDirections() == 1);

            foreach (var cell in leafCells)
                cell.Type = CellType.Sink;

            // Trim the sinks until it's below the max
            var shuffledSinks = rng.Shuffle(puzzle.AllSinks()).ToArray();
            int numSinksToTrim = shuffledSinks.Length - numSinks;
            for (int i = 0; i < numSinksToTrim; i++)
            {
                puzzle.RemoveCell(shuffledSinks[i]);
            }

            // Trim all branches that belonged to the trimmed sinks
            while (AllNonSinkLeaves().Count() > 0)
            {
                var cellsToRemove = AllNonSinkLeaves().ToArray();
                foreach (var cellPos in cellsToRemove)
                    puzzle.RemoveCell(cellPos);
            }

            // Scramble the puzzle to hide the solution
            foreach (var cellPos in puzzle.AllCellsReachableFrom(source))
            {
                var cell = puzzle.GetCell(cellPos);

                int numRotations = rng.Next(0, 4);
                for (int i = 0; i < numRotations; i++)
                    cell.RotateClockwise();
            }

            return puzzle;

            IEnumerable<(Vector2i cellPos, CardinalDirection dir)> GrowthCandidates()
            {
                var reachableCells = puzzle.AllCellsReachableFrom(source).ToArray();

                foreach (var cellPos in reachableCells)
                {
                    // Pipe cells can only have 3 ports max.
                    // If there were four ports, then rotating it would do nothing,
                    // and the puzzle could be too easy.
                    var cell = puzzle.GetCell(cellPos);
                    if (cell.Type == CellType.Pipe && cell.NumOpenDirections() >= 3)
                        continue;

                    foreach (var dir in PossibleGrowthDirections(cellPos))
                        yield return (cellPos, dir);
                }
            }

            IEnumerable<CardinalDirection> PossibleGrowthDirections(Vector2i cellPos)
            {
                var cell = puzzle.GetCell(cellPos);

                foreach (var dir in CardinalDirectionUtils.All())
                {
                    var neighborPos = cellPos + dir.ToVector2i();
                    if (!puzzle.IsInBounds(neighborPos))
                        continue;

                    if (cell.IsDirectionOpen(dir))
                        continue;

                    var neighborCell = puzzle.GetCell(neighborPos);
                    if (neighborCell.Type != CellType.Empty)
                        continue;

                    yield return dir;
                }
            }

            IEnumerable<Vector2i> AllNonSinkLeaves()
            {
                return puzzle._cells
                    .Where(kvp => kvp.Value.NumOpenDirections() == 1 && kvp.Value.Type != CellType.Sink)
                    .Select(kvp => kvp.Key);
            }
        }

        public PipePuzzleGraph(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public Cell GetCell(Vector2i pos)
        {
            AssertInBounds(pos);

            if (!_cells.ContainsKey(pos))
                _cells[pos] = new Cell();

            return _cells[pos];
        }

        public bool IsSolved()
        {
            IEnumerable<Vector2i> sources = _cells
                .Where(kvp => kvp.Value.Type == CellType.Source)
                .Select(kvp => kvp.Key);

            IEnumerable<Vector2i> sinks = _cells
                .Where(kvp => kvp.Value.Type == CellType.Sink)
                .Select(kvp => kvp.Key);

            HashSet<Vector2i> reachableCells = sources
                .SelectMany(src => AllCellsReachableFrom(src))
                .ToHashSet();

            return sinks.All(sink => reachableCells.Contains(sink));
        }

        public bool IsInBounds(Vector2i cellPos)
        {
            return
                cellPos.x >= 0 &&
                cellPos.y >= 0 &&
                cellPos.x < Width &&
                cellPos.y < Height;
        }

        public IEnumerable<Vector2i> AllCellsReachableFrom(Vector2i startPos)
        {
            var alreadyQueued = new HashSet<Vector2i>();
            var visitQueue = new Queue<Vector2i>();

            EnqueueIfNotAlready(startPos);

            while (visitQueue.Any())
            {
                var currentPos = visitQueue.Dequeue();
                yield return currentPos;

                foreach (Vector2i neighborPos in AllConnectedNeighbors(currentPos))
                {
                    EnqueueIfNotAlready(neighborPos);
                }
            }

            void EnqueueIfNotAlready(Vector2i pos)
            {
                if (alreadyQueued.Contains(pos))
                    return;

                alreadyQueued.Add(pos);
                visitQueue.Enqueue(pos);
            }
        }

        public IEnumerable<Vector2i> AllConnectedNeighbors(Vector2i cellPos)
        {
            var cell = GetCell(cellPos);

            foreach (var dir in CardinalDirectionUtils.All())
            {
                var neighborPos = cellPos + dir.ToVector2i();

                if (!IsInBounds(neighborPos))
                    continue;

                var neighborCell = GetCell(neighborPos);

                bool cellOpen = cell.IsDirectionOpen(dir);
                bool neighborOpen = neighborCell.IsDirectionOpen(dir.Opposite());

                if (cellOpen && neighborOpen)
                    yield return neighborPos;
            }
        }

        public IEnumerable<Vector2i> AllPipes()
        {
            return _cells
                .Where(kvp => kvp.Value.Type == CellType.Pipe)
                .Select(kvp => kvp.Key);
        }

        public IEnumerable<Vector2i> AllSources()
        {
            return _cells
                .Where(kvp => kvp.Value.Type == CellType.Source)
                .Select(kvp => kvp.Key);
        }

        public IEnumerable<Vector2i> AllSinks()
        {
            return _cells
                .Where(kvp => kvp.Value.Type == CellType.Sink)
                .Select(kvp => kvp.Key);
        }

        public void RemoveCell(Vector2i cellPos)
        {
            // Close the ports on all neighbors
            var cell = GetCell(cellPos);
            foreach (var dir in CardinalDirectionUtils.All())
            {
                if (!cell.IsDirectionOpen(dir))
                    continue;

                var neighborPos = cellPos + dir.ToVector2i();
                var neighborCell = GetCell(neighborPos);
            }
        }

        private void AssertInBounds(Vector2i cellPos)
        {
            if (!IsInBounds(cellPos))
                throw new Exception($"Cell {cellPos} is out of bounds");
        }

        public enum CellType
        {
            Empty,
            Pipe,
            Source,
            Sink
        }

        public class Cell
        {
            public CellType Type;

            private Dictionary<CardinalDirection, bool> _isDirectionOpen = new Dictionary<CardinalDirection, bool>();

            public void RotateClockwise()
            {
                bool tmp = IsDirectionOpen(CardinalDirection.West);
                SetDirectionOpen(CardinalDirection.West, IsDirectionOpen(CardinalDirection.South));
                SetDirectionOpen(CardinalDirection.South, IsDirectionOpen(CardinalDirection.East));
                SetDirectionOpen(CardinalDirection.East, IsDirectionOpen(CardinalDirection.North));
                SetDirectionOpen(CardinalDirection.North, tmp);
            }

            public bool IsDirectionOpen(CardinalDirection dir)
            {
                bool hasValue = _isDirectionOpen.TryGetValue(dir, out bool isOpen);
                return hasValue && isOpen;
            }

            public int NumOpenDirections()
            {
                return CardinalDirectionUtils.All()
                    .Where(IsDirectionOpen)
                    .Count();
            }

            public void SetDirectionOpen(CardinalDirection dir, bool isOpen)
            {
                _isDirectionOpen[dir] = isOpen;
            }
        }
    }
}
