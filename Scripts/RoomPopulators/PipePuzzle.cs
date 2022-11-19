using System;
using System.Collections.Generic;
using Godot;

namespace RandomDungeons
{
    public class PipePuzzle : Node2D, IRoomPopulator
    {
        [Export] public int Width = 10;
        [Export] public int Height = 10;
        [Export] public int NumSinks = 3;
        [Export] public int MinGrowths = 15;

        [Export] public PackedScene SourceCellPrefab;
        [Export] public PackedScene PipeCellPrefab;
        [Export] public PackedScene SinkCellPrefab;

        public void Populate(DungeonGraphRoom graphRoom, Random rng)
        {
            var puzzleGraph = PipePuzzleGraph.Generate(
                rng: rng,
                width: Width,
                height: Height,
                numSinks: NumSinks,
                minGrowths: MinGrowths
            );

            foreach (var srcPos in puzzleGraph.AllSources())
            {
                Spawn<Node2D>(SourceCellPrefab, srcPos);
            }

            foreach (var pipePos in puzzleGraph.AllPipes())
            {
                Spawn<Node2D>(PipeCellPrefab, pipePos);
            }

            foreach (var sinkPos in puzzleGraph.AllSinks())
            {
                Spawn<Node2D>(SinkCellPrefab, sinkPos);
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
            return new Vector2(graphPos.x, graphPos.y) * 32;
        }
    }
}
