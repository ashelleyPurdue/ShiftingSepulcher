using Godot;

using RandomDungeons.PhysicalPuzzles;
using RandomDungeons.Graphs;

namespace RandomDungeons.Maps
{
    public class LightsOutTestMap : Node
    {
        [Export] public PackedScene VictoryScreen;

        private LightsOutPuzzle _physicalPuzzle
            => GetNode<LightsOutPuzzle>("%LightsOutPuzzle");

        public override void _Ready()
        {
            var graph = LightsOutGraph.Generate(
                seed: TitleScreen.ChosenSeed,
                width: 3,
                height: 3,
                numFlips: 10
            );

            _physicalPuzzle.SetGraph(graph);
            _physicalPuzzle.Solved += OnPuzzleSolved;
        }

        private void OnPuzzleSolved()
        {
            GetTree().ChangeSceneTo(VictoryScreen);
        }
    }
}
