using System;
using System.Linq;
using Godot;

namespace RandomDungeons
{
    public class WarpTrigger : Area2D
    {
        /// <summary>
        /// The path to the Room2D that this warp takes you to.
        /// This is only used for hand-crafted dungeons.
        /// </summary>
        [Export] public NodePath TargetRoomPath;
        [Export] public string TargetEntrance;

        public Room2D TargetRoom;

        private int _ignoreBodyEnteredTimer;

        public override void _Ready()
        {
            // This signal is connected with the "deferred" flag, so it won't
            // actually trigger until the frame _after_ the player enters the
            // trigger.  This is because Godot forbids messing with the scene
            // tree during signal processing.
            Connect(
                signal: "body_entered",
                target: this,
                method: nameof(BodyEntered),
                flags: (int)ConnectFlags.Deferred
            );
        }

        public override void _EnterTree()
        {
            // Ignore the BodyEntered signal for a few physics frames.
            //
            // This is to prevent the player from getting stuck in an infinite
            // warping loop if a room transition places them directly on top of
            // another warp trigger.
            //
            // It needs to be more than one frame because, for some reason,
            // Area2D's don't start polling for overlapping bodies until a few
            // frames after it's added to the tree.
            _ignoreBodyEnteredTimer = 3;
        }

        public override void _PhysicsProcess(float delta)
        {
            if (_ignoreBodyEnteredTimer > 0)
                _ignoreBodyEnteredTimer--;
        }

        private void BodyEntered(object body)
        {
            if (_ignoreBodyEnteredTimer > 0)
                return;

            if (body is Player)
            {
                RoomTransitionManager.Instance.EnterRoom(
                    TargetRoom,
                    TargetEntrance,
                    GlobalPosition
                );
            }
        }
    }
}
