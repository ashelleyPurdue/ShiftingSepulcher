using Godot;

using RandomDungeons.PhysicalDungeons;
using RandomDungeons.Nodes.Elements;
using RandomDungeons.Graphs;

namespace RandomDungeons.Nodes.Maps
{
    public class LightsOutTestMap : Node
    {
        private LightsOutPuzzle _puzzle => GetNode<LightsOutPuzzle>("%LightsOutPuzzle");
        private DoorBars _bars => GetNode<DoorBars>("%DoorBars");

        public override void _Ready()
        {
            var graph = LightsOutGraph.Generate(
                seed: TitleScreen.ChosenSeed,
                width: 3,
                height: 3,
                numFlips: 10
            );

            _puzzle.SetGraph(graph);
            _bars.Challenge = _puzzle;
        }
    }
}
