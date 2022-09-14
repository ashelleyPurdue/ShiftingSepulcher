using Godot;
using RandomDungeons.Graphs;
using RandomDungeons.Nodes.Elements;
using RandomDungeons.PhysicalDungeons;
using RandomDungeons.Utils;

namespace RandomDungeons.Nodes.DungeonRooms
{
    public class SlidingIcePuzzleRoom : SimpleDungeonRoom
    {
        [Export] public PackedScene DoorBarsPrefab;
        [Export] public PackedScene SlidingIcePuzzlePrefab;

        private Node2D _contentSpawn => GetNode<Node2D>("%ContentSpawn");

        public override void Populate(DungeonGraphRoom graphRoom)
        {
            base.Populate(graphRoom);

            var graph = SlidingIceGraph.Generate(
                seed: GraphRoom.RoomSeed,
                width: 10,
                height: 10,
                numPushes: 5,
                numRedHerringRocks: 3
            );
            var puzzle = Create<SlidingIcePuzzle>(_contentSpawn, SlidingIcePuzzlePrefab);
            puzzle.SetGraph(graph);

            // TODO: Only spawn bars on the main "forward" door; don't spawn
            // them on "side" doors or the "go back" door.
            SpawnDoorBars(CardinalDirection.North, puzzle);
            SpawnDoorBars(CardinalDirection.South, puzzle);
            SpawnDoorBars(CardinalDirection.East, puzzle);
            SpawnDoorBars(CardinalDirection.West, puzzle);
        }

        private void SpawnDoorBars(CardinalDirection dir, IDungeonRoomChallenge challenge)
        {
            var parent = GetDoorSpawn(dir);
            var graphDoor = GraphRoom.GetDoor(dir);

            // Don't spawn bars on plain walls
            if (graphDoor.Destination == null)
                return;

            var bars = Create<DoorBars>(parent, DoorBarsPrefab);
            bars.Challenge = challenge;
        }
    }
}
