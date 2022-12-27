using System;
using System.Linq;
using Godot;

namespace RandomDungeons
{
    public class DoorWarp : Node2D
    {
        public Room2D TargetRoom;
        public string TargetEntrance;

        public event Action DoorUsed;

        public override void _EnterTree()
        {
            // To prevent the player from rapidly "jittering" between two rooms
            // during a transition, the warp trigger is disabled until the
            // player passes through an "enabling" trigger in front of the door.
            EnableWarp(false);
        }

        private void EnableWarp(bool enable)
        {
            GetNode<Area2D>("%WarpTrigger").Monitoring = enable;
        }

        // This signal is connected with the "deferred" flag, so it won't
        // actually trigger until the frame _after_ the player enters the
        // trigger.  This is because Godot forbids messing with the scene
        // tree during signal processing.
        private void WarpTriggerBodyEntered(object body)
        {
            if (body is Player)
            {
                DoorUsed?.Invoke();
            }
        }

        // This signal is _also_ conencted with the "deferred" flag.
        // This is to avoid a "Can't change this state while flushing queries".
        //
        // tl;dr: Godot forbids messing with collision data from within a
        // collision notification, and EnableWarp() messes with collision data
        // (specifically, it disables a collision shape).
        //
        // Rules, rules, rules...
        private void WarpEnableTriggerBodyEntered(object body)
        {
            if (body is Player)
                EnableWarp(true);
        }
    }
}
