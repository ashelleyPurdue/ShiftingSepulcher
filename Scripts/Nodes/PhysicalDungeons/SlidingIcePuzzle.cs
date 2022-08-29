using System;
using Godot;

using RandomDungeons.Graphs;
using RandomDungeons.Utils;

namespace RandomDungeons.PhysicalDungeons
{
    public class SlidingIcePuzzle : Node2D
    {
        [Export] public PackedScene IceBlockPrefab;
        [Export] public PackedScene RockPrefab;
        [Export] public PackedScene EndingSlotPrefab;
        [Export] public PackedScene DirtPatchPrefab;

        private TextureRect _floor => GetNode<TextureRect>("%Floor");
        private Node2D _puzzleElements => GetNode<Node2D>("%PuzzleElements");

        public void SetGraph(SlidingIceGraph graph)
        {
            // Resize the floor to match the graph's size
            _floor.MarginRight = graph.Width * 32;
            _floor.MarginBottom = graph.Width * 32;

            // TODO: Clear out existing children

            // Add the surrounding dirt-patches
            // TODO: Use fewer objects
            for (int x = 0; x <= graph.Width; x++)
            {
                Create(DirtPatchPrefab, new Vector2i(x, -1));
                Create(DirtPatchPrefab, new Vector2i(x, graph.Height + 1));
            }
            for (int y = 0; y <= graph.Height; y++)
            {
                Create(DirtPatchPrefab, new Vector2i(-1, y));
                Create(DirtPatchPrefab, new Vector2i(graph.Width + 1, y));
            }

            // Place the ice block and goal
            Create(IceBlockPrefab, graph.StartPos);
            Create(EndingSlotPrefab, graph.EndPos);

            // Place all the rocks
            foreach (var rockPos in graph.RockPositions)
            {
                Create(RockPrefab, rockPos);
            }
        }

        private void Create(PackedScene prefab, Vector2i pos)
        {
            var node = prefab.Instance<Node2D>();
            _puzzleElements.AddChild(node);
            node.Position = ToRealPos(pos);
        }

        private Vector2 ToRealPos(Vector2i puzzlePos)
        {
            return new Vector2(puzzlePos.x, puzzlePos.y) * 32;
        }
    }
}
