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

        public override void Populate(DungeonGraphRoom graphRoom)
        {
            base.Populate(graphRoom);

            var graph = LightsOutGraph.Generate(
                seed: GraphRoom.RoomSeed,
                width: 4,
                height: 4,
                numFlips: 3
            );
            var puzzle = Create<LightsOutPuzzle>(_contentSpawn, LightsOutPuzzlePrefab);
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

            var bars = Create<DoorBars>(parent, DoorPrefabs.Bars);
            bars.Challenge = challenge;
        }
    }
}
