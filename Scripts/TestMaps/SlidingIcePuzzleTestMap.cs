using System;
using Godot;

namespace RandomDungeons
{
    public class SlidingIcePuzzleTestMap : Node2D
    {
        private SlidingIcePuzzle _puzzle => GetNode<SlidingIcePuzzle>("%SlidingIcePuzzle");
        private ChallengeDungeonGraphDoor _graphDoor = new ChallengeDungeonGraphDoor();

        private DoorBars _bars => GetNode<DoorBars>("%DoorBars");

        public override void _Ready()
        {
            var treeRoom = new DungeonTreeRoom();
            treeRoom.RoomSeed = TitleScreen.ChosenSeed;

            var rng = new Random(TitleScreen.ChosenSeed);
            _puzzle.Populate(treeRoom, rng);
        }

        public override void _PhysicsProcess(float delta)
        {
            _bars.IsOpened =_puzzle.IsSolved();
        }
    }
}
