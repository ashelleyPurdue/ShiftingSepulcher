using Godot;

namespace RandomDungeons.Nodes.Elements
{
    public class SlidingIceBlock : Node2D
    {
        private const float SlideSpeed = 32 * 10;

        private RayCast2D _wallDetector => GetNode<RayCast2D>("%WallDetector");
        private CollisionShape2D _collider => GetNode<CollisionShape2D>("%Collider");

        private Vector2 _velocity;
        private State _currentState = State.Resting;
        private enum State
        {
            Resting,
            Sliding
        }

        public override void _PhysicsProcess(float delta)
        {
            switch (_currentState)
            {
                case State.Sliding:
                {
                    if (_wallDetector.IsColliding())
                    {
                        StartResting();
                        break;
                    }

                    Position += _velocity * delta;
                    break;
                }
            }
        }

        private void StartResting()
        {
            _currentState = State.Resting;
            _velocity = Vector2.Zero;
            _collider.Disabled = false;
            _wallDetector.Enabled = false;

            SnapToGrid();
        }

        private void SnapToGrid()
        {
            var pos = Position;
            pos.x = Mathf.RoundToInt(pos.x / 32) * 32;
            pos.y = Mathf.RoundToInt(pos.y / 32) * 32;

            Position = pos;
        }

        private void Push(Vector2 dir)
        {
            dir = dir.Normalized();

            if (_currentState != State.Resting)
                return;

            _currentState = State.Sliding;
            _velocity = dir * SlideSpeed;
            _collider.Disabled = true;

            _wallDetector.Enabled = true;
            _wallDetector.CastTo = dir;
            _wallDetector.Position = new Vector2(16, 16) + (dir * 16);
            _wallDetector.ForceRaycastUpdate();
        }

        private void OnPushedLeft() => Push(Vector2.Left);
        private void OnPushedRight() => Push(Vector2.Right);
        private void OnPushedDown() => Push(Vector2.Down);
        private void OnPushedUp() => Push(Vector2.Up);
    }
}

