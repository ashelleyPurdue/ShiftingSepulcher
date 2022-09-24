using Godot;
using RandomDungeons.Graphs;
using RandomDungeons.PhysicalDungeons;

namespace RandomDungeons.Nodes.DungeonRooms
{
    public class SlidingIcePuzzleRoom : SimpleDungeonRoom
    {
        [Export] public PackedScene SlidingIcePuzzlePrefab;

        private Node2D _contentSpawn => GetNode<Node2D>("%ContentSpawn");
        private SlidingIcePuzzle _puzzle;

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
            _puzzle = Create<SlidingIcePuzzle>(_contentSpawn, SlidingIcePuzzlePrefab);
            _puzzle.SetGraph(graph);
        }

        public override bool IsChallengeSolved()
        {
            return _puzzle.IsSolved();
        }
    }
}
