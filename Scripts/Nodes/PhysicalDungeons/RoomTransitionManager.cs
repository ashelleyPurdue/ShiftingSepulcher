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
        private AnimationPlayer _transitionAnimator => GetNode<AnimationPlayer>("%RoomTransitionAnimator");
        private Node2D _activeRoomHolder => GetNode<Node2D>("%ActiveRoomHolder");
        private Node2D _previousRoomHolder => GetNode<Node2D>("%PreviousRoomHolder");
        private Camera2D _camera => GetNode<Camera2D>("%Camera");

        private Dictionary<DungeonGraphRoom, IDungeonRoom> _graphRoomToRealRoom;
        private Dictionary<IDungeonRoom, DungeonGraphRoom> _realRoomToGraphRoom;

        private IDungeonRoom _activeRoom;
        private IDungeonRoom _prevRoom;

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

        public void RemovePreviousRoom()
        {
            _transitionAnimator.Stop();

            if (_prevRoom == null)
                return;

            _previousRoomHolder.RemoveChild(_prevRoom.Node);
            _prevRoom = null;
        }

        private void OnDoorUsed(CardinalDirection dir)
        {
            if (_activeRoom == null)
                throw new Exception("How did you use a door?  There's no active room!");

            DungeonGraphRoom nextGraphRoom = _realRoomToGraphRoom[_activeRoom]
                .GetDoor(dir)
                .Destination;

            IDungeonRoom nextRoom = _graphRoomToRealRoom[nextGraphRoom];

            var prevDoorSpawn = _activeRoom.GetDoorSpawn(dir);
            var nextDoorSpawn = nextRoom.GetDoorSpawn(dir.Opposite());

            Vector2 nextRoomPos = prevDoorSpawn.GlobalPosition - nextDoorSpawn.Position;
            nextRoomPos += dir.ToVector2() * 32;

            EnterRoom(nextGraphRoom, nextRoomPos);
        }

        private void EnterRoom(DungeonGraphRoom graphRoom, Vector2 position)
        {
            IDungeonRoom room = _graphRoomToRealRoom[graphRoom];

            if (_activeRoom == room)
                return;

            RemovePreviousRoom();
            SetPreviousRoom(_activeRoom);
            SetActiveRoom(room);

            room.Node.GlobalPosition = position;
            _camera.GlobalPosition = position;
            _transitionAnimator.Play("Fade");
        }

        private void SetPreviousRoom(IDungeonRoom room)
        {
            if (_prevRoom == room)
                return;

            ReparentNode(room.Node, _previousRoomHolder);
            room.Node.SetPaused(true);
            _prevRoom = room;
        }

        private void SetActiveRoom(IDungeonRoom room)
        {
            if (_activeRoom == room)
                return;

            ReparentNode(room.Node, _activeRoomHolder);
            room.Node.SetPaused(false);
            _activeRoom = room;
        }

        private void ReparentNode(Node node, Node newParent)
        {
            node.GetParent()?.RemoveChild(node);
            newParent.AddChild(node);
        }
    }
}
