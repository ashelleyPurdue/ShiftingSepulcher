using System.Linq;
using Godot;
using RandomDungeons.DungeonGraphs;

namespace RandomDungeons.PhysicalDungeons
{
    public class SquareRoomDoor : Node2D
    {
        public DungeonDoor GraphDoor;

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
    }
}
