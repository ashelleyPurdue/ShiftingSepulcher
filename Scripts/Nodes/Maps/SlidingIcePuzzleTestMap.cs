using Godot;

using RandomDungeons.Graphs;
using RandomDungeons.PhysicalDungeons;

namespace RandomDungeons.Nodes.Maps
{
    public class SlidingIcePuzzleTestMap : Node2D
    {
        public override void _Ready()
        {
            var graph = SlidingIceGraph.Generate(
                seed: 1337,
                width: 10,
                height: 10,
                numPushes: 3
            );

            GetNode<SlidingIcePuzzle>("%SlidingIcePuzzle").SetGraph(graph);
        }
    }
}
