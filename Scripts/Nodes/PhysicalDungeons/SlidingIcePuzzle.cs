using System;
using Godot;

using RandomDungeons.Graphs;
using RandomDungeons.Utils;
using RandomDungeons.Nodes.Elements;

namespace RandomDungeons.PhysicalDungeons
{
    public class SlidingIcePuzzle : Node2D, IDungeonRoomChallenge
    {
        [Export] public PackedScene IceBlockPrefab;
        [Export] public PackedScene RockPrefab;
        [Export] public PackedScene EndingSlotPrefab;
        [Export] public PackedScene DirtPatchPrefab;

        private TextureRect _floor => GetNode<TextureRect>("%Floor");
        private Node2D _puzzleElements => GetNode<Node2D>("%PuzzleElements");
        private Node2D _northBorder => GetNode<Node2D>("%NorthBorder");
        private Node2D _southBorder => GetNode<Node2D>("%SouthBorder");
        private Node2D _eastBorder => GetNode<Node2D>("%EastBorder");
        private Node2D _westBorder => GetNode<Node2D>("%WestBorder");
        private Node2D _puzzleOffset => GetNode<Node2D>("%PuzzleOffset");

        private SlidingIceBlock _iceBlock;

        private SlidingIceGraph _graph;

        public void SetGraph(SlidingIceGraph graph)
        {
            _graph = graph;
            ResetPuzzle();
        }

        public void ResetPuzzle()
        {
            // Remove all existing elements
            while (_puzzleElements.GetChildCount() > 0)
            {
                var child = _puzzleElements.GetChild(0);
                _puzzleElements.RemoveChild(child);
                child.QueueFree();
            }

            // Resize the floor to match the graph's size
            _floor.MarginRight = _graph.Width * 32;
            _floor.MarginBottom = _graph.Width * 32;

            // Add the surrounding dirt-patches
            _northBorder.Scale = new Vector2(_graph.Width, 1);
            _southBorder.Scale = new Vector2(_graph.Width, 1);
            _eastBorder.Scale  = new Vector2(1, _graph.Height);
            _westBorder.Scale  = new Vector2(1, _graph.Height);

            _southBorder.Position = new Vector2(0, _graph.Height) * 32;
            _eastBorder.Position  = new Vector2(_graph.Width, 0) * 32;

            // Place the ice block and goal
            Create<Node2D>(EndingSlotPrefab, _graph.EndPos);
            _iceBlock = Create<SlidingIceBlock>(IceBlockPrefab, _graph.StartPos);

            // Place all the rocks
            foreach (var rockPos in _graph.RockPositions)
            {
                Create<Node2D>(RockPrefab, rockPos);
            }

            // Shift everything so the center of the puzzle matches the center
            // of this node
            _puzzleOffset.Position = new Vector2(_graph.Width, _graph.Height) * -16;
        }

        public bool IsSolved()
        {
            if (_graph == null)
                return false;

            Vector2 endPos = ToRealPos(_graph.EndPos);
            float endPosDist = _iceBlock.Position.DistanceTo(endPos);

            return
                endPosDist < 0.01f &&
                !_iceBlock.IsSliding;
        }

        private T Create<T>(PackedScene prefab, Vector2i pos) where T : Node2D
        {
            var node = prefab.Instance<T>();
            _puzzleElements.AddChild(node);
            node.Position = ToRealPos(pos);

            return node;
        }

        private Vector2 ToRealPos(Vector2i puzzlePos)
        {
            return new Vector2(puzzlePos.x, puzzlePos.y) * 32;
        }
    }
}
