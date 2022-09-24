using Godot;
using RandomDungeons.Graphs;
using RandomDungeons.Nodes.Elements;
using RandomDungeons.PhysicalDungeons;
using RandomDungeons.Utils;

namespace RandomDungeons.Nodes.DungeonRooms
{
    public class SlidingIcePuzzleRoom : SimpleDungeonRoom
    {
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

            SpawnDoorBars(puzzle);
        }
    }
}
