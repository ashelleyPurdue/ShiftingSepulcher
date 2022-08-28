using Godot;

namespace RandomDungeons.Nodes.Elements
{
    public class SlidingIceBlock : KinematicBody2D
    {
        private Vector2 _velocity;

        public override void _Process(float delta)
        {
            var collision = MoveAndCollide(_velocity);
            if (collision != null)
            {
                _velocity = Vector2.Zero;
            }
        }

        private void Push(Vector2 dir)
        {
            _velocity = dir.Normalized() * 5;
        }

        private void OnPushedLeft() => Push(Vector2.Left);
        private void OnPushedRight() => Push(Vector2.Right);
        private void OnPushedDown() => Push(Vector2.Down);
        private void OnPushedUp() => Push(Vector2.Up);
    }
}

