using Godot;

namespace RandomDungeons
{
    public class DoorBars : Node2D
    {
        public const float OpenCloseTime = 0.1f;

        public bool IsOpened = false;

        private Node2D _visuals => GetNode<Node2D>("%Visuals");

        public override void _Process(float delta)
        {
            GetNode<CollisionShape2D>("%CollisionShape").Disabled = IsOpened;

            var targetScale = IsOpened
                ? new Vector2(0, 1)
                : new Vector2(1, 1);

            float speed = 1f / OpenCloseTime;
            _visuals.Scale = _visuals.Scale.MoveToward(targetScale, speed * delta);
        }
    }
}
