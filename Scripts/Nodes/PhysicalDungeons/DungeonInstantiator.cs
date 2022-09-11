using System.Collections.Generic;
using System.Linq;
using Godot;
using RandomDungeons.Graphs;
using RandomDungeons.Nodes.Maps;
using RandomDungeons.Utils;

namespace RandomDungeons.PhysicalDungeons
{
    public class DungeonInstantiator : Node
    {
        private const float FadeTime = 0.25f;

        private DungeonRoomFactory _roomFactory => GetNode<DungeonRoomFactory>("%RoomFactory");

        private Dictionary<DungeonGraphRoom, IDungeonRoom> _graphRoomToRealRoom
            = new Dictionary<DungeonGraphRoom, IDungeonRoom>();

        private IDungeonRoom _activeRoom;
        private IDungeonRoom _disappearingRoom;

        public override void _Ready()
        {
            // Generate a dungeon graph
            GD.Print(TitleScreen.ChosenSeed);

            var graph = DungeonGenerator.GenerateUsingRuns(
                seed: TitleScreen.ChosenSeed,
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
                var realRoom = _roomFactory.BuildRoom(graphRoom);
                _graphRoomToRealRoom[graphRoom] = realRoom;

                realRoom.DoorUsed += OnDoorUsed;
            }

            // Start in the starting room
            EnterRoom(graph.StartRoom);
        }

        public override void _Process(float deltaTime)
        {
            float fadeSpeed = 1 / FadeTime;

            // Fade the new room in
            _activeRoom.FadePercent = Mathf.MoveToward(
                _activeRoom.FadePercent,
                1,
                deltaTime * fadeSpeed
            );

            // Fade the old room out
            if (_disappearingRoom != null)
            {
                _disappearingRoom.FadePercent = Mathf.MoveToward(
                    _disappearingRoom.FadePercent,
                    0,
                    deltaTime * fadeSpeed
                );

                if (_disappearingRoom.FadePercent <= 0)
                    FinishFadingOut();
            }
        }

        private void OnDoorUsed(CardinalDirection dir)
        {
            DungeonGraphRoom nextGraphRoom = _activeRoom
                .GraphRoom
                .GetDoor(dir)
                .Destination;

            var prevRoom = _activeRoom;
            var nextRoom = _graphRoomToRealRoom[nextGraphRoom];

            if (prevRoom != null)
            {
                var prevDoorSpawn = prevRoom.GetDoorSpawn(dir);
                var nextDoorSpawn = nextRoom.GetDoorSpawn(dir.Opposite());

                nextRoom.Position = prevDoorSpawn.GlobalPosition - nextDoorSpawn.Position;
            }

            EnterRoom(nextGraphRoom);
        }

        public void EnterRoom(DungeonGraphRoom graphRoom)
        {
            var prevRoom = _activeRoom;
            var nextRoom = _graphRoomToRealRoom[graphRoom];

            if (nextRoom == _disappearingRoom)
            {
                GD.Print("Trying to enter a room that's already fading out");
                _activeRoom = nextRoom;
                _disappearingRoom = null;

                MoveCameraToActiveRoom();
            }

            if (nextRoom == prevRoom)
            {
                GD.Print("Trying to enter a room we're already in");
                return;
            }

            StartFadingOut(prevRoom);
            StartFadingIn(nextRoom);
            MoveCameraToActiveRoom();
        }

        private void StartFadingIn(IDungeonRoom room)
        {
            if (room.GetParent() != this)
            {
                room.GetParent()?.RemoveChild(room);
                AddChild(room);
            }

            room.FadePercent = 0;
            _activeRoom = room;
        }

        private void StartFadingOut(IDungeonRoom room)
        {
            if (room == null)
                return;

            // There can only be one room fading out at a time.
            // If there already is one, skip to the end of it so the next
            // one can start fading out.
            FinishFadingOut();

            room.FadePercent = 1;
            _disappearingRoom = room;
        }

        private void FinishFadingOut()
        {
            if (_disappearingRoom == null)
                return;

            _disappearingRoom.FadePercent = 0;
            RemoveChild(_disappearingRoom);
            _disappearingRoom = null;
        }

        private void MoveCameraToActiveRoom()
        {
            var camera = GetTree()
                .GetNodesInGroup("Camera")
                .Cast<Camera2D>()
                .First();

            camera.GlobalPosition = _activeRoom.GlobalPosition;
        }
    }
}
