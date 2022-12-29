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
        private Node2D _nextRoomHolder => GetNode<Node2D>("%NextRoomHolder");

        private Node2D _previousRoomTexture => GetNode<Node2D>("%PreviousRoomTexture");
        private Node2D _nextRoomTexture => GetNode<Node2D>("%NextRoomTexture");

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
            // Go back to the start room
            UnparentNode(_activeRoom);
            ReparentNode(_startRoom, _activeRoomHolder);
            _activeRoom = _startRoom;
            _activeRoomHolder.GlobalPosition = Vector2.Zero;

            _camera.GlobalPosition = Vector2.Zero;

            // Resurrect the player
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

            // Restart the music
            MusicService.Instance.PlaySong(BackgroundMusic);
        }

        public void EnterRoom(
            Room2D room,
            string entranceName,
            Vector2 position
        )
        {
            var entrance = room.GetEntrance(entranceName);
            position -= GetRelativePosition(room, entrance);

            if (_activeRoom == room)
                return;

            GetTree().FindPlayer().ReleaseHeldObject();

            // Notify nodes that the room is being entered.
            // Puzzles can listen for this and reset themselves when you re-enter
            // the room, for example.
            foreach (var node in room.AllDescendantsOfType<IOnRoomEnter>())
            {
                node.OnRoomEnter();
            }

            // Make the active room the previous room, and put it in the viewport
            _prevRoom = _activeRoom;
            ReparentNode(_prevRoom, _previousRoomHolder);
            _previousRoomTexture.GlobalPosition = _activeRoomHolder.GlobalPosition;

            // Put the next room in the other viewport
            _activeRoom = room;
            ReparentNode(_activeRoom, _nextRoomHolder);
            _nextRoomTexture.GlobalPosition = position;

            // Freeze the previous room, and unfreeze the next room
            _activeRoom.SetPaused(false);
            _prevRoom.SetPaused(true);

            // Play the transition animation, now that it's been set up
            _transitionAnimator.ResetAndPlay("Fade");

            _camera.GlobalPosition = position;
        }

        private void TransitionAnimationFinished()
        {
            _activeRoomHolder.GlobalPosition = _nextRoomTexture.GlobalPosition;
            ReparentNode(_activeRoom, _activeRoomHolder);
            UnparentNode(_prevRoom);
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

        private void ReparentNode(Node node, Node newParent)
        {
            node.GetParent()?.RemoveChild(node);
            newParent.AddChild(node);
        }

        private void UnparentNode(Node node)
        {
            node?.GetParent()?.RemoveChild(node);
        }
    }
}
