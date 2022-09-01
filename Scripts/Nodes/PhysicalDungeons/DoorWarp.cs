using System;
using System.Linq;
using Godot;

using RandomDungeons.Graphs;
using RandomDungeons.Nodes.Elements;

namespace RandomDungeons.PhysicalDungeons
{
    public class DoorWarp : Node2D
    {
        private DungeonDoor _graphDoor;

        public void SetGraphDoor(DungeonDoor graphDoor)
        {
            _graphDoor = graphDoor;
        }

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

            // There's an invisible wall that exists while the warp trigger is
            // disabled, to prevent the player from simply walking out of bounds.
            GetNode<CollisionShape2D>("%WarpDisabledGuard").Disabled = enable;
        }

        // This signal is connected with the "deferred" flag, so it won't
        // actually trigger until the frame _after_ the player enters the
        // trigger.  This is because Godot forbids messing with the scene
        // tree during signal processing.
        private void WarpTriggerBodyEntered(object body)
        {
            if (body is Player)
            {
                var instantiator = GetTree()
                    .GetNodesInGroup("DungeonInstantiator")
                    .Cast<DungeonInstantiator>()
                    .First();

                instantiator.EnterRoom(_graphDoor.Destination);
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
