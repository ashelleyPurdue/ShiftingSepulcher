using System.Collections.Generic;
using System.Linq;
using Godot;
using RandomDungeons.DungeonGraphs;

namespace RandomDungeons.PhysicalDungeons
{
    public class DungeonInstantiator : Node
    {
        private Dictionary<DungeonRoom, SquareRoom> _graphRoomToRealRoom
            = new Dictionary<DungeonRoom, SquareRoom>();

        private SquareRoom _activeRoom;

        public override void _Ready()
        {
            var roomPrefab = GD.Load<PackedScene>("res://Prefabs/SquareRoom.tscn");

            // Generate a dungeon graph
            GD.Randomize();
            int seed = (int)GD.Randi();
            GD.Print(seed);

            var graph = DungeonGenerator.GenerateUsingRuns(
                seed: seed,
                minRunSize: 3,
                maxRunSize: 5,
                numRooms: 25
            );

            // Create a "real" version of each room, but don't add it to the
            // scene yet.  We'll add it to the scene later, when the player
            // actually _enters_ it.
            foreach (var coordinates in graph.AllRoomCoordinates())
            {
                var graphRoom = graph.GetRoom(coordinates);
                var realRoom = roomPrefab.Instance<SquareRoom>();

                realRoom.GraphRoom = graphRoom;
                _graphRoomToRealRoom[graphRoom] = realRoom;
            }

            // Start in the starting room
            EnterRoom(graph.StartRoom);
        }

        public void EnterRoom(DungeonRoom graphRoom)
        {
            // Unload the current room
            if (_activeRoom != null)
                RemoveChild(_activeRoom);

            // Load in the replacement
            _activeRoom = _graphRoomToRealRoom[graphRoom];
            AddChild(_activeRoom);

            // Yank the camera over here
            var camera = GetTree()
                .GetNodesInGroup("Camera")
                .Cast<Camera2D>()
                .First();

            camera.GlobalPosition = _activeRoom.GlobalPosition;
        }
    }
}
