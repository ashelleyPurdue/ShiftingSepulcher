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

        private void EnterRoom(DungeonGraphRoom room, Vector2 position)
        {
            // TODO: Deal with edge cases
            RemovePreviousRoom();

            if (_activeRoom != null)
            {
                _activeRoomHolder.RemoveChild(_activeRoom.Node);
                _previousRoomHolder.AddChild(_activeRoom.Node);
                _prevRoom = _activeRoom;

                SetNodePaused(_prevRoom.Node, true);
            }

            _activeRoom = _graphRoomToRealRoom[room];
            _activeRoom.Node.GlobalPosition = position;

            SetNodePaused(_activeRoom.Node, false);
            _activeRoomHolder.AddChild(_activeRoom.Node);

            _camera.GlobalPosition = _activeRoom.Node.GlobalPosition;
            _transitionAnimator.Play("Fade");
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
