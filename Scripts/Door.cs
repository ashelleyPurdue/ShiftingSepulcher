using Godot;
using RandomDungeons.DungeonGraphs;

namespace RandomDungeons
{
    public class Door : Node2D
    {
        public DungeonDoor GraphDoor;

        public override void _Process(float delta)
        {
            // Hide or show the door
            var transform = Transform;

            transform.Scale = GraphDoor.Destination != null && !GraphDoor.IsLocked
                ? Vector2.Zero
                : Vector2.One;

            Transform = transform;

            // Display which lock is on this door
            var label = GetNode<Label>("Label");
            GetNode<Label>("Label").Text = GraphDoor.IsLocked
                ? $"Lock {GraphDoor.LockId}"
                : "";
        }
    }
}
