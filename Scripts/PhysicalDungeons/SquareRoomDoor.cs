using System.Linq;
using Godot;
using RandomDungeons.DungeonGraphs;

namespace RandomDungeons.PhysicalDungeons
{
    public class SquareRoomDoor : Node2D
    {
        public DungeonDoor GraphDoor;

        public override void _Process(float delta)
        {
            // Hide or show the door
            var transform = GetNode<Node2D>("%Door").Transform;

            transform.Scale = GraphDoor.Destination != null && !GraphDoor.IsLocked
                ? Vector2.Zero
                : Vector2.One;

            GetNode<Node2D>("%Door").Transform = transform;

            // Display which lock is on this door
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
