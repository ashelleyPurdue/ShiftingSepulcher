using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace RandomDungeons
{
    public class RoomTransitionManager : Node
    {
        [Export] public AudioStream BackgroundMusic;

        private AnimationPlayer _transitionAnimator => GetNode<AnimationPlayer>("%RoomTransitionAnimator");
        private Node2D _activeRoomHolder => GetNode<Node2D>("%ActiveRoomHolder");
        private Node2D _previousRoomHolder => GetNode<Node2D>("%PreviousRoomHolder");
        private Camera2D _camera => GetNode<Camera2D>("%Camera");

        private Dictionary<DungeonGraphRoom, Room2D> _graphRoomToRealRoom;
        private Dictionary<Room2D, DungeonGraphRoom> _realRoomToGraphRoom;
        private DungeonGraphRoom _startRoom;

        private Room2D _activeRoom;
        private Room2D _prevRoom;

        public void SetGraph(
            DungeonGraph graph,
            Dictionary<DungeonGraphRoom, IDungeonRoom> graphRoomToRealRoom
        )
        {
            PlayerInventory.Reset();

            _startRoom = graph.StartRoom;
            _graphRoomToRealRoom = graphRoomToRealRoom.ToDictionary(
                kvp => kvp.Key,
                kvp => (Room2D)kvp.Value.Node
            );
            _realRoomToGraphRoom = _graphRoomToRealRoom.Invert();

            foreach (var realRoom in graphRoomToRealRoom.Values)
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

            realStartRoom.GlobalPosition = Vector2.Zero;
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
                foreach (var enemy in room.AllDescendantsOfType<IRespawnable>())
                {
                    enemy.Respawn();
                }
            }

            MusicService.Instance.PlaySong(BackgroundMusic);
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

            _previousRoomHolder.RemoveChild(_prevRoom);
            _prevRoom = null;
        }

        private void OnDoorUsed(CardinalDirection dir)
        {
            if (_activeRoom == null)
                throw new Exception("How did you use a door?  There's no active room!");

            DungeonGraphRoom nextGraphRoom = _realRoomToGraphRoom[_activeRoom]
                .GetDoor(dir)
                .Destination;

            Room2D nextRoom = _graphRoomToRealRoom[nextGraphRoom];

            var prevEntrance = _activeRoom.GetEntrance(dir.ToString());
            var nextEntrance = nextRoom.GetEntrance(dir.Opposite().ToString());

            Vector2 offset = GetRelativePosition(nextRoom, nextEntrance);
            Vector2 nextRoomPos = prevEntrance.GlobalPosition - offset;
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
            Room2D room = _graphRoomToRealRoom[graphRoom];

            if (_activeRoom == room)
                return;

            GetTree().FindPlayer().ReleaseHeldObject();

            RemovePreviousRoom();
            SetPreviousRoom(_activeRoom);
            SetActiveRoom(room);

            room.GlobalPosition = position;
            _camera.GlobalPosition = position;
            _transitionAnimator.Play("Fade");

            // Notify nodes that the room is being entered.
            // Puzzles can listen for this and reset themselves when you re-enter
            // the room, for example.
            foreach (var node in room.AllDescendantsOfType<IOnRoomEnter>())
            {
                node.OnRoomEnter();
            }
        }

        private void SetPreviousRoom(Room2D room)
        {
            if (_prevRoom == room)
                return;

            ReparentNode(room, _previousRoomHolder);
            room.SetPaused(true);
            _prevRoom = room;
        }

        private void SetActiveRoom(Room2D room)
        {
            if (_activeRoom == room)
                return;

            ReparentNode(room, _activeRoomHolder);
            room.SetPaused(false);
            _activeRoom = room;
        }

        private void ReparentNode(Node node, Node newParent)
        {
            node.GetParent()?.RemoveChild(node);
            newParent.AddChild(node);
        }
    }
}
