using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace RandomDungeons
{
    public class RoomTransitionManager : Node
    {
        public static RoomTransitionManager Instance {get; private set;}

        [Export] public AudioStream BackgroundMusic;

        private AnimationPlayer _transitionAnimator => GetNode<AnimationPlayer>("%RoomTransitionAnimator");
        private Node2D _activeRoomHolder => GetNode<Node2D>("%ActiveRoomHolder");
        private Node2D _previousRoomHolder => GetNode<Node2D>("%PreviousRoomHolder");
        private Camera2D _camera => GetNode<Camera2D>("%Camera");

        private Room2D _startRoom;
        private Room2D _activeRoom;
        private Room2D _prevRoom;

        private IEnumerable<Room2D> _roomsToRespawn;

        public override void _Ready()
        {
            Instance = this;
        }

        public void StartDungeon(
            Room2D startRoom,
            IEnumerable<Room2D> roomsToRespawn
        )
        {
            _startRoom = startRoom;
            _roomsToRespawn = roomsToRespawn;

            PlayerInventory.Reset();
            RespawnPlayer();
        }

        public void RespawnPlayer()
        {
            if (_activeRoom == _startRoom)
            {
                RespawnTransitionFinished();
                return;
            }

            RemovePreviousRoom();
            SetPreviousRoom(_activeRoom);
            RemovePreviousRoom();
            SetActiveRoom(_startRoom);

            _startRoom.GlobalPosition = Vector2.Zero;
            _camera.GlobalPosition = Vector2.Zero;
            _transitionAnimator.Play("Respawn");
        }

        public void EnterRoom(
            Room2D room,
            string entranceName,
            Vector2 position
        )
        {
            if (_activeRoom == room)
                return;

            GetTree().FindPlayer().ReleaseHeldObject();

            RemovePreviousRoom();
            SetPreviousRoom(_activeRoom);
            SetActiveRoom(room);

            var entrance = room.GetEntrance(entranceName);
            position -= GetRelativePosition(room, entrance);

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

        public void RespawnTransitionFinished()
        {
            // Respawn the player
            var player = GetTree().FindPlayer();
            player.GlobalPosition = Vector2.Zero;
            player.Resurrect();

            // Respawn all the enemies
            foreach (var room in _roomsToRespawn)
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

        private Vector2 GetRelativePosition(Node2D parent, Node2D descendant)
        {
            bool isParentInTree = parent.IsInsideTree();

            if (!isParentInTree)
                AddChild(parent);

            Vector2 result = descendant.GlobalPosition - parent.GlobalPosition;

            if (!isParentInTree)
                RemoveChild(parent);

            return result;
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
