using System;
using System.Collections.Generic;
using Godot;

namespace RandomDungeons
{
    public class RoomTransitionManager : Node
    {
        private AnimationPlayer _transitionAnimator => GetNode<AnimationPlayer>("%RoomTransitionAnimator");
        private Node2D _activeRoomHolder => GetNode<Node2D>("%ActiveRoomHolder");
        private Node2D _previousRoomHolder => GetNode<Node2D>("%PreviousRoomHolder");
        private Camera2D _camera => GetNode<Camera2D>("%Camera");

        private Dictionary<DungeonGraphRoom, IDungeonRoom> _graphRoomToRealRoom;
        private Dictionary<IDungeonRoom, DungeonGraphRoom> _realRoomToGraphRoom;
        private DungeonGraphRoom _startRoom;

        private IDungeonRoom _activeRoom;
        private IDungeonRoom _prevRoom;

        public void SetGraph(
            DungeonGraph graph,
            Dictionary<DungeonGraphRoom, IDungeonRoom> graphRoomToRealRoom
        )
        {
            PlayerInventory.Reset();

            _startRoom = graph.StartRoom;
            _graphRoomToRealRoom = graphRoomToRealRoom;
            _realRoomToGraphRoom = graphRoomToRealRoom.Invert();

            foreach (var realRoom in _graphRoomToRealRoom.Values)
            {
                realRoom.DoorUsed += OnDoorUsed;
            }

            RespawnPlayer();
        }

        public void RespawnPlayer()
        {
            var realStartRoom = _graphRoomToRealRoom[_startRoom];

            if (_activeRoom == realStartRoom)
            {
                RespawnTransitionFinished();
                return;
            }

            RemovePreviousRoom();
            SetPreviousRoom(_activeRoom);
            RemovePreviousRoom();
            SetActiveRoom(realStartRoom);

            realStartRoom.Node.GlobalPosition = Vector2.Zero;
            _camera.GlobalPosition = Vector2.Zero;
            _transitionAnimator.Play("Respawn");
        }

        public void RespawnTransitionFinished()
        {
            // Respawn the player
            var player = GetTree().FindPlayer();
            player.GlobalPosition = Vector2.Zero;
            player.Resurrect();

            // Respawn all the enemies
            foreach (var room in _realRoomToGraphRoom.Keys)
            {
                foreach (var enemy in room.Node.AllDescendantsOfType<IRespawnable>())
                {
                    enemy.Respawn();
                }
            }
        }

        public override void _Process(float delta)
        {
            // HACK: Kill the player when a button is pressed
            bool isPressed = Input.IsKeyPressed((int)KeyList.R);

            if (isPressed && !_wasPressed)
                PlayerInventory.Health = 0;

            _wasPressed = isPressed;
        }
        private bool _wasPressed = false;

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

            Vector2 offset = GetRelativePosition(nextRoom.Node, nextDoorSpawn);
            Vector2 nextRoomPos = prevDoorSpawn.GlobalPosition - offset;
            nextRoomPos -= offset.Normalized() * 32;

            EnterRoom(nextGraphRoom, nextRoomPos);
        }

        private Vector2 GetRelativePosition(Node2D parent, Node2D descendant)
        {
            bool isParentInTree = parent.IsInsideTree();

            if (!isParentInTree)
                AddChild(parent);

            Vector2 result = descendant.GlobalPosition - parent.Position;

            if (!isParentInTree)
                RemoveChild(parent);

            return result;
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
