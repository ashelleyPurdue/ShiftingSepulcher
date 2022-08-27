using Godot;

using RandomDungeons.PhysicalPuzzles;
using RandomDungeons.PuzzleGraphs;

namespace RandomDungeons.TestMaps
{
    public class LightsOutTestMap : Node
    {
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
            GD.Print("Set graph");
        }
    }
}
