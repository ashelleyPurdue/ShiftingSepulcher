using Godot;
using RandomDungeons.Graphs;
using RandomDungeons.Nodes.Elements;
using RandomDungeons.PhysicalDungeons;
using RandomDungeons.Utils;

namespace RandomDungeons.Nodes.DungeonRooms
{
    public class LightsOutRoom : SimpleDungeonRoom
    {
        [Export] public PackedScene LightsOutPuzzlePrefab;

        private Node2D _contentSpawn => GetNode<Node2D>("%ContentSpawn");
        private LightsOutPuzzle _puzzle;

        public override void Populate(DungeonGraphRoom graphRoom)
        {
            base.Populate(graphRoom);

            var graph = LightsOutGraph.Generate(
                seed: GraphRoom.RoomSeed,
                width: 4,
                height: 4,
                numFlips: 3
            );
            _puzzle = Create<LightsOutPuzzle>(_contentSpawn, LightsOutPuzzlePrefab);
            _puzzle.SetGraph(graph);

            SpawnDoorBars();
        }

        public override void _PhysicsProcess(float delta)
        {
            foreach (var door in ChallengeDoors())
            {
                door.IsOpened = _puzzle.IsSolved();
            }
        }
    }
}
