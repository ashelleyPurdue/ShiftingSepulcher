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
                seed: TitleScreen.ChosenSeed,
                width: 10,
                height: 10,
                numPushes: 5,
                numRedHerringRocks: 3
            );

            GetNode<SlidingIcePuzzle>("%SlidingIcePuzzle").SetGraph(graph);
        }
    }
}
