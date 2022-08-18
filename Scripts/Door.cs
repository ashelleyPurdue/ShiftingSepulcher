using Godot;

namespace RandomDungeons
{
    public class Door : Node2D
    {
        public bool IsOpen = false;

        public override void _Process(float delta)
        {
            var transform = Transform;

            transform.Scale = IsOpen
                ? Vector2.Zero
                : Vector2.One;

            Transform = transform;
        }
    }
}