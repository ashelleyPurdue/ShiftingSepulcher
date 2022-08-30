using Godot;

using RandomDungeons.Graphs;
using RandomDungeons.PhysicalDungeons;

namespace RandomDungeons.Nodes.Maps
{
    public class SlidingIcePuzzleTestMap : Node2D
    {
        [Export] public PackedScene VictoryScreen;

        private SlidingIcePuzzle _puzzle => GetNode<SlidingIcePuzzle>("%SlidingIcePuzzle");

        public override void _Ready()
        {
            var graph = SlidingIceGraph.Generate(
                seed: TitleScreen.ChosenSeed,
                width: 10,
                height: 10,
                numPushes: 5,
                numRedHerringRocks: 3
            );

            _puzzle.SetGraph(graph);
        }

        public override void _Process(float delta)
        {
            if (_puzzle.IsSolved())
                GetTree().ChangeSceneTo(VictoryScreen);
        }
    }
}
