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

                parentCell.SetDirectionOpen(growth.dir, true);
                parentCell.SetDirectionOpen(growth.dir.Opposite(), true);
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
                    bool isFree = neighborCell.NumOpenDirections() == 0;

                    if (!isFree)
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


        // placeholders for intellisense
        public Cell GetCell(Vector2i cellPos) => throw new NotImplementedException();
        public void RemoveCell(Vector2i cellPos) => throw new NotImplementedException();
        public bool IsInBounds(Vector2i cellPos) => throw new NotImplementedException();
        public IEnumerable<Vector2i> AllSinks() => throw new NotImplementedException();
        public IEnumerable<Vector2i> AllCellsReachableFrom(Vector2i startPos) => throw new NotImplementedException();

        public enum CellType
        {
            Pipe,
            Source,
            Sink
        }

        public class Cell
        {
            public CellType Type;

            public void SetDirectionOpen(CardinalDirection dir, bool isOpen) => throw new NotImplementedException();
            public int NumOpenDirections() => throw new NotImplementedException();
            public void RotateClockwise() => throw new NotImplementedException();
            public bool IsDirectionOpen(CardinalDirection dir) => throw new NotImplementedException();
        }
    }
}
