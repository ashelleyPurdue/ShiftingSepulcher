using Godot;

using RandomDungeons.Graphs;
using RandomDungeons.PhysicalDungeons;
using RandomDungeons.Nodes.Elements;
using RandomDungeons.Nodes.UI;

namespace RandomDungeons.Nodes.Maps
{
    public class SlidingIcePuzzleTestMap : Node2D
    {
        private SlidingIcePuzzle _puzzle => GetNode<SlidingIcePuzzle>("%SlidingIcePuzzle");
        private ChallengeDungeonGraphDoor _graphDoor = new ChallengeDungeonGraphDoor();

        private DoorBars _bars => GetNode<DoorBars>("%DoorBars");

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
            _bars.SetGraphDoor(_graphDoor);
        }

        public override void _PhysicsProcess(float delta)
        {
            _graphDoor.IsOpened =_puzzle.IsSolved();
        }
    }
}
