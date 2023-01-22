using System;
using System.Collections.Generic;
using System.Linq;

namespace RandomDungeons
{
    public class PipePuzzleGraph
    {
        public readonly int Width;
        public readonly int Height;

        public Vector2i MinCoords => new Vector2i(
            x: AllNonEmptyCells().Select(c => c.x).Min(),
            y: AllNonEmptyCells().Select(c => c.y).Min()
        );

        public Vector2i MaxCoords => new Vector2i(
            x: AllNonEmptyCells().Select(c => c.x).Max(),
            y: AllNonEmptyCells().Select(c => c.y).Max()
        );

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
            puzzle.CreateCell(source, CellType.Source);

            // Choose a bunch of cells to "grow" another cell off of
            for (int i = 0; i < minGrowths || AllLeafPipes().Count() < numSinks; i++)
            {
                var growthCandidates = GrowthCandidates().ToArray();

                if (growthCandidates.Length == 0)
                    break;

                var growth = rng.PickFrom(growthCandidates);

                var parentPos = growth.cellPos;
                var parentCell = puzzle.GetCell(parentPos);

                var childPos = parentPos + growth.dir.ToVector2i();
                var childCell = puzzle.CreateCell(childPos, CellType.Pipe);

                parentCell.SetDirectionOpen(growth.dir, true);
                childCell.SetDirectionOpen(growth.dir.Opposite(), true);
            }

            // Mark all "leaf" cells as sinks
            var leafCells = puzzle._cells
                .Select(kvp => kvp.Value)
                .Where(c => c.Type != CellType.Source)
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
            while (AllLeafPipes().Count() > 0)
            {
                var cellsToRemove = AllLeafPipes().ToArray();
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

                    if (!puzzle.IsCellEmpty(neighborPos))
                        continue;

                    yield return dir;
                }
            }

            IEnumerable<Vector2i> AllLeafPipes()
            {
                return puzzle._cells
                    .Where(kvp => kvp.Value.Type == CellType.Pipe)
                    .Where(kvp => kvp.Value.NumOpenDirections() == 1)
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
            return _cells[pos];
        }

        public Cell CreateCell(Vector2i pos, CellType type)
        {
            AssertInBounds(pos);
            var cell = new Cell(type);
            _cells[pos] = cell;

            return cell;
        }

        public bool IsSolved()
        {
            HashSet<Vector2i> reachableCells = AllSources()
                .SelectMany(src => AllCellsReachableFrom(src))
                .ToHashSet();

            return AllSinks().All(sink => reachableCells.Contains(sink));
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

        private IEnumerable<Vector2i> AllConnectedNeighbors(Vector2i cellPos)
        {
            var cell = GetCell(cellPos);

            foreach (var dir in CardinalDirectionUtils.All())
            {
                var neighborPos = cellPos + dir.ToVector2i();

                if (!IsInBounds(neighborPos))
                    continue;

                if (IsCellEmpty(neighborPos))
                    continue;

                var neighborCell = GetCell(neighborPos);

                bool cellOpen = cell.IsDirectionOpen(dir);
                bool neighborOpen = neighborCell.IsDirectionOpen(dir.Opposite());

                if (cellOpen && neighborOpen)
                    yield return neighborPos;
            }
        }

        public bool IsCellEmpty(Vector2i pos) => !_cells.ContainsKey(pos);

        public IEnumerable<Vector2i> AllNonEmptyCells()
        {
            return _cells.Select(kvp => kvp.Key);
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
                neighborCell.SetDirectionOpen(dir.Opposite(), false);
            }

            // Remove the cell
            _cells.Remove(cellPos);
        }

        private void AssertInBounds(Vector2i cellPos)
        {
            if (!IsInBounds(cellPos))
                throw new Exception($"Cell {cellPos} is out of bounds");
        }

        public enum CellType
        {
            Pipe,
            Source,
            Sink
        }

        public class Cell
        {
            public CellType Type;

            private Dictionary<CardinalDirection, bool> _isDirectionOpen = new Dictionary<CardinalDirection, bool>();

            public Cell(CellType type)
            {
                Type = type;
            }

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
