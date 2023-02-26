using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace ShiftingSepulcher
{
    public class PipePuzzle : Node2D, IRoomPopulator, IChallenge
    {
        [Export] public int Width = 5;
        [Export] public int Height = 5;
        [Export] public int NumSinks = 3;
        [Export] public int MinGrowths = 15;

        [Export] public PackedScene SourceCellPrefab;
        [Export] public PackedScene PipeCellPrefab;
        [Export] public PackedScene SinkCellPrefab;

        private PipePuzzleGraph _puzzleGraph;
        private const float CellSize = 32 * 3;

        public bool IsSolved() => _puzzleGraph.IsSolved();

        public void Populate(DungeonTreeRoom treeRoom, Random rng)
        {
            var puzzleGraph = PipePuzzleGraph.Generate(
                rng: rng,
                width: Width,
                height: Height,
                numSinks: NumSinks,
                minGrowths: MinGrowths
            );
            _puzzleGraph = puzzleGraph;

            // Create nodes for all the cells in the graph
            foreach (var srcPos in puzzleGraph.AllSources())
            {
                var src = Spawn<PipePuzzleCell>(SourceCellPrefab, srcPos);
                src.Cell = puzzleGraph.GetCell(srcPos);
            }

            foreach (var pipePos in puzzleGraph.AllPipes())
            {
                var pipe = Spawn<PipePuzzleCell>(PipeCellPrefab, pipePos);
                pipe.Cell = puzzleGraph.GetCell(pipePos);
            }

            foreach (var sinkPos in puzzleGraph.AllSinks())
            {
                var sink = Spawn<PipePuzzleCell>(SinkCellPrefab, sinkPos);
                sink.Cell = puzzleGraph.GetCell(sinkPos);
            }

            // HACK: Move the chest spawn point to one of the empty cells
            var chestSpawn = GetNode<Node2D>("%ChestSpawns/Center");
            chestSpawn.Position = rng.PickFrom(PotentialChestSpawns());

            // React when the cells get rotated
            foreach (var cell in this.AllDescendantsOfType<PipePuzzleCell>())
            {
                cell.CellRotated += OnCellRotated;
            }

            OnCellRotated();
        }

        private void OnCellRotated()
        {
            var allPoweredGraphCells = _puzzleGraph
                .AllSources()
                .SelectMany(srcPos => _puzzleGraph.AllCellsReachableFrom(srcPos))
                .Select(cellPos => _puzzleGraph.GetCell(cellPos))
                .ToHashSet();

            foreach (var realCell in this.AllDescendantsOfType<PipePuzzleCell>())
            {
                realCell.IsPowered = allPoweredGraphCells.Contains(realCell.Cell);
                realCell.UpdatePipeDisplays();
            }
        }

        private TNode Spawn<TNode>(PackedScene prefab, Vector2i graphPos) where TNode : Node2D
        {
            var node = prefab.Instance<TNode>();
            node.Position = GraphPosToRealPos(graphPos);
            AddChild(node);

            return node;
        }

        private Vector2 GraphPosToRealPos(Vector2i graphPos)
        {
            // Make it so the center of the _used_ tiles is at (0,0)
            Vector2 usedCenter = _puzzleGraph.MinCoords.ToVector2() + _puzzleGraph.MaxCoords.ToVector2();
            usedCenter /= 2;

            var realPos = graphPos.ToVector2();
            realPos -= usedCenter;
            realPos.y *= -1;

            // Scale it up by the size of a cell
            realPos *= CellSize;

            return realPos;
        }

        private IEnumerable<Vector2> PotentialChestSpawns()
        {
            foreach (Vector2i emptyCellPos in _puzzleGraph.AllEmptyCells())
            {
                var realPos = GraphPosToRealPos(emptyCellPos);

                // Just because a cell is empty doesn't mean we can put a
                // chest there.  It's possible that the cell's "real" position
                // is inside a wall, due to GraphPosToRealPos() shifting everything
                // to put the used cells in the center.
                //
                // So, we need to check if the _real_ position is within the
                // bounds of the room.
                Vector2 realMax = new Vector2(_puzzleGraph.Width, _puzzleGraph.Height);
                realMax *= CellSize;
                realMax /= 2;

                Vector2 realMin = -realMax;

                bool inBounds =
                    realPos.x < realMax.x &&
                    realPos.x >= realMin.x &&
                    realPos.y < realMax.y &&
                    realPos.y >= realMin.y;

                if (inBounds)
                    yield return realPos;
            }
        }
    }
}
