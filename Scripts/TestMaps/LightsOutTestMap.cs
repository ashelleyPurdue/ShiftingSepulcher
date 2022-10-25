using System;
using Godot;

namespace RandomDungeons
{
    public class LightsOutTestMap : Node
    {
        private LightsOutPuzzle _puzzle => GetNode<LightsOutPuzzle>("%LightsOutPuzzle");
        private DoorBars _bars => GetNode<DoorBars>("%DoorBars");
        private ChallengeDungeonGraphDoor _graphDoor = new ChallengeDungeonGraphDoor();

        public override void _Ready()
        {
            var graphRoom = new DungeonGraphRoom(
                new DungeonGraph(),
                Vector2i.Zero,
                0
            );
            graphRoom.RoomSeed = TitleScreen.ChosenSeed;

            var rng = new Random(TitleScreen.ChosenSeed);
            _puzzle.Populate(graphRoom, rng);
            _bars.SetGraphDoor(_graphDoor);
        }

        public override void _PhysicsProcess(float delta)
        {
            _graphDoor.IsOpened =_puzzle.IsSolved();
        }
    }
}
