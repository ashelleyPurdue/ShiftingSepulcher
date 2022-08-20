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
        private SquareRoom _disappearingRoom;

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

        public override void _Process(float deltaTime)
        {
            // Make the active room gradually fade in
            // TODO: Make it gradual
            // TODO: Actually _fade_ it, instead of growing it.
            _activeRoom.Scale = Vector2.One;

            // Make the previous room gradually fade out
            // TODO: Actually _fade_ it, instead of shrinking it.
            if (_disappearingRoom != null)
            {
                _disappearingRoom.Scale -= Vector2.One * deltaTime * 4;

                if (_disappearingRoom.Scale.x < 0)
                {
                    _disappearingRoom.Scale = Vector2.Zero;
                    RemoveChild(_disappearingRoom);
                    _disappearingRoom = null;
                }
            }
        }

        public void EnterRoom(DungeonRoom graphRoom)
        {
            // We only support one room "fading out" at a time.
            // If another room is still fading out, skip straight to the end of
            // it so the next one can start.
            if (_disappearingRoom != null)
            {
                RemoveChild(_disappearingRoom);
                _disappearingRoom = null;
            }

            // Make the previous room start disappearing
            _disappearingRoom = _activeRoom;

            // Load in the new room
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
