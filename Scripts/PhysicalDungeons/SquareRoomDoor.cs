using System.Linq;
using Godot;
using RandomDungeons.DungeonGraphs;

namespace RandomDungeons.PhysicalDungeons
{
    public class SquareRoomDoor : Node2D
    {
        public DungeonDoor GraphDoor;

        public override void _EnterTree()
        {
            // To prevent the player from rapidly "jittering" between two rooms
            // during a transition, the warp trigger is disabled until the
            // player passes through an "enabling" trigger in front of the door.
            EnableWarp(false);
        }

        public override void _Process(float deltaTime)
        {
            UpdateDoorOpen(deltaTime);
            UpdateLockDisplay(deltaTime);
        }

        private void UpdateDoorOpen(float deltaTime)
        {
            var door = GetNode<Node2D>("%Door");

            var transform = door.Transform;

            transform.Scale = GraphDoor.Destination != null && !GraphDoor.IsLocked
                ? Vector2.Zero
                : Vector2.One;

            door.Transform = transform;
        }

        private void UpdateLockDisplay(float deltaTime)
        {
            var label = GetNode<Label>("Label");
            GetNode<Label>("Label").Text = GraphDoor.IsLocked
                ? $"Lock {GraphDoor.LockId}"
                : "";
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

                instantiator.EnterRoom(GraphDoor.Destination);
            }
        }

        private void WarpEnableTriggerBodyEntered(object body)
        {
            if (body is Player)
                EnableWarp(true);
        }
    }
}
