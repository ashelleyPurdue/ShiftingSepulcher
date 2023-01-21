using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace RandomDungeons
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
        }

        public override void _PhysicsProcess(float delta)
        {
            var allPoweredGraphCells = _puzzleGraph
                .AllSources()
                .SelectMany(srcPos => _puzzleGraph.AllCellsReachableFrom(srcPos))
                .Select(cellPos => _puzzleGraph.GetCell(cellPos))
                .ToHashSet();

            foreach (var realCell in this.AllDescendantsOfType<PipePuzzleCell>())
            {
                realCell.IsPowered = allPoweredGraphCells.Contains(realCell.Cell);
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
            realPos *= 32 * 3;

            return realPos;
        }
    }
}
