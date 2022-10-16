using System;
using System.Collections.Generic;
using Godot;

using RandomDungeons.Graphs;
using RandomDungeons.Nodes.DungeonRooms;
using RandomDungeons.Utils;

namespace RandomDungeons.PhysicalDungeons
{
    public class RoomTransitionManager : Node
    {
        private Camera2D _camera => GetNode<Camera2D>("%Camera");

        private Dictionary<DungeonGraphRoom, IDungeonRoom> _graphRoomToRealRoom;
        private Dictionary<IDungeonRoom, DungeonGraphRoom> _realRoomToGraphRoom;

        private IDungeonRoom _activeRoom;

        public void SetGraph(
            DungeonGraph graph,
            Dictionary<DungeonGraphRoom, IDungeonRoom> graphRoomToRealRoom
        )
        {
            _graphRoomToRealRoom = graphRoomToRealRoom;
            _realRoomToGraphRoom = graphRoomToRealRoom.Invert();
            
            foreach (var realRoom in _graphRoomToRealRoom.Values)
            {
                realRoom.DoorUsed += OnDoorUsed;
            }

            EnterRoom(graph.StartRoom, Vector2.Zero);
        }

        private void OnDoorUsed(CardinalDirection dir)
        {
            if (_activeRoom == null)
                throw new Exception("How did you use a door?  There's no active room!");

            DungeonGraphRoom nextGraphRoom = _realRoomToGraphRoom[_activeRoom];

            IDungeonRoom prevRoom = _activeRoom;
            IDungeonRoom nextRoom = _graphRoomToRealRoom[nextGraphRoom];

            var prevDoorSpawn = prevRoom.GetDoorSpawn(dir);
            var nextDoorSpawn = nextRoom.GetDoorSpawn(dir.Opposite());

            Vector2 nextRoomPos = prevDoorSpawn.GlobalPosition - nextDoorSpawn.GlobalPosition;
            nextRoomPos += dir.ToVector2() * 32;

            EnterRoom(nextGraphRoom, nextRoomPos);
        }

        private void EnterRoom(DungeonGraphRoom room, Vector2 position)
        {
            // TODO: Pause the previous room
            // TODO: Start the animation fading the previous room out
            // TODO: Deal with edge cases

            if (_activeRoom != null)
                RemoveChild(_activeRoom.Node);
            
            _activeRoom = _graphRoomToRealRoom[room];
            _activeRoom.Node.GlobalPosition = position;
            AddChild(_activeRoom.Node);

            _camera.GlobalPosition = _activeRoom.Node.GlobalPosition;
        }

        private void SetNodePaused(Node node, bool paused)
        {
            node.SetProcess(!paused);
            node.SetPhysicsProcess(!paused);
            node.SetProcessInput(!paused);
            node.SetProcessUnhandledInput(!paused);
            node.SetProcessUnhandledKeyInput(!paused);

            foreach (var child in node.GetChildren())
            {
                SetNodePaused((Node)child, paused);
            }
        }
    }
}