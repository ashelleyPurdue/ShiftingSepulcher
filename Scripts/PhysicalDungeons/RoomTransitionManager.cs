using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace RandomDungeons
{
    public enum RoomTransitionAnimation
    {
        Fade,
        StairsUp,
        StairsDown
    }
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

        private Node2D _previousRoomTextureScaler => GetNode<Node2D>("%PreviousRoomTextureScaler");
        private Node2D _nextRoomTextureScaler => GetNode<Node2D>("%NextRoomTextureScaler");

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
            Vector2 position,
            RoomTransitionAnimation anim = RoomTransitionAnimation.Fade
        )
        {
            var entrance = room.GetEntrance(entranceName);
            position -= GetRelativePosition(room, entrance.Node);

            if (_activeRoom == room)
                return;

            // Notify nodes that the room is being entered.
            // Puzzles can listen for this and reset themselves when you re-enter
            // the room, for example.
            foreach (var node in room.AllDescendantsOfType<IOnRoomEnter>())
            {
                node.OnRoomEnter();
            }

            // Freeze the player during the transition, to prevent them from
            // taking advantage of the fact that the walls are intangible during
            // the transition animation.
            var player = GetTree().FindPlayer();
            player.ControlsEnabled = false;

            // Force the player to drop whatever object they're holding, to
            // prevent them from carrying objects between rooms.
            // Otherwise, players would be able to cheat at puzzles.
            player.ReleaseHeldObject();

            // Put the next and previous rooms inside separate viewports, so they
            // can't interact with things during the transition animation.
            // The next room will be moved back to the "main" viewport after the
            // transition animation is finished.
            //
            // A viewport is like a "pocket dimension" with its own isolated
            // physics.  Physics colliders can only interact with each other if
            // they're inside the same viewport.
            // Make the active room the previous room, and put it in the viewport
            _prevRoom = _activeRoom;
            _activeRoom = room;

            ReparentNode(_prevRoom, _previousRoomHolder);
            ReparentNode(_activeRoom, _nextRoomHolder);
            _activeRoom.Position = Vector2.Zero;

            // Move the render-textures for both rooms into the correct
            // positions for the start of the animation.
            //
            // Because the next and previous rooms aren't in the "main" viewport
            // during the transition, they won't appear on-screen by default.
            // Instead, they get rendered to textures, which then get displayed
            // on two giant rectangles.  These rectangles are what get manipulated
            // during the transition animation.
            _previousRoomTextureScaler.Scale = Vector2.One;
            _nextRoomTextureScaler.Scale = Vector2.One;

            _previousRoomTextureScaler.GlobalPosition = player.GlobalPosition;
            _nextRoomTextureScaler.GlobalPosition = player.GlobalPosition;

            _previousRoomTexture.GlobalPosition = _activeRoomHolder.GlobalPosition;
            _nextRoomTexture.GlobalPosition = position;

            // Freeze the previous room, and unfreeze the next room
            _activeRoom.SetPaused(false);
            _prevRoom.SetPaused(true);

            // Play the transition animation, now that it's been set up
            _transitionAnimator.ResetAndPlay(anim.ToString());
            _camera.GlobalPosition = position;
        }

        private void TransitionAnimationFinished()
        {
            _activeRoomHolder.GlobalPosition = _nextRoomTexture.GlobalPosition;
            ReparentNode(_activeRoom, _activeRoomHolder);
            UnparentNode(_prevRoom);

            var player = GetTree().FindPlayer();
            player.ControlsEnabled = true;
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
