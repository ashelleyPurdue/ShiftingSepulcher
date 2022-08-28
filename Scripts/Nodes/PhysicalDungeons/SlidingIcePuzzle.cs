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

        public void SetGraph(SlidingIceGraph graph)
        {
            // TODO: Clear out existing children
            // TODO: Add the surrounding walls
            // TODO: Add the ending pos

            Create(IceBlockPrefab, graph.StartPos);

            foreach (var rockPos in graph.RockPositions)
            {
                Create(RockPrefab, rockPos);
            }
        }

        private void Create(PackedScene prefab, Vector2i pos)
        {
            var node = prefab.Instance<Node2D>();
            AddChild(node);
            node.Position = ToRealPos(pos);
        }

        private Vector2 ToRealPos(Vector2i puzzlePos)
        {
            return new Vector2(puzzlePos.x, puzzlePos.y) * 32;
        }
    }
}
