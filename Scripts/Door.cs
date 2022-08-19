using Godot;
using RandomDungeons.DungeonGraphs;

namespace RandomDungeons
{
    public class Door : Node2D
    {
        public DungeonDoor GraphDoor;

        public override void _Process(float delta)
        {
            var transform = Transform;

            transform.Scale = GraphDoor.Destination != null
                ? Vector2.Zero
                : Vector2.One;

            Transform = transform;
        }
    }
}
