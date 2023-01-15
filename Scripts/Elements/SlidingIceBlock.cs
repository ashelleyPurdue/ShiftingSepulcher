using Godot;

namespace RandomDungeons
{
    public class SlidingIceBlock : Node2D
    {
        private const float SlideSpeed = 32 * 10;

        public bool IsSliding {get; private set;}

        private RayCast2D _wallDetector => GetNode<RayCast2D>("%WallDetector");
        private CollisionShape2D _collider => GetNode<CollisionShape2D>("%Collider");

        private AudioStreamPlayer _slideStartSound => GetNode<AudioStreamPlayer>("%SlideStartSound");
        private AudioStreamPlayer _slideLoopSound => GetNode<AudioStreamPlayer>("%SlideLoopSound");
        private AudioStreamPlayer _slideStopSound => GetNode<AudioStreamPlayer>("%SlideStopSound");

        private Vector2 _velocity;

        public override void _PhysicsProcess(float delta)
        {
            if (!IsSliding)
                return;

            if (WouldHitWall(_velocity * delta))
            {
                StopSliding();
                return;
            }

            Position += _velocity * delta;
        }

        private bool WouldHitWall(Vector2 deltaPos)
        {
            var dir = deltaPos.Normalized();

            _wallDetector.Position = new Vector2(16, 16) + (dir * 16);
            _wallDetector.CastTo = deltaPos;
            _wallDetector.ForceRaycastUpdate();

            return _wallDetector.IsColliding();
        }

        private void StopSliding()
        {
            IsSliding = false;
            _velocity = Vector2.Zero;
            _collider.Disabled = false;
            _wallDetector.Enabled = false;

            SnapToGrid();

            _slideLoopSound.Stop();
            _slideStopSound.Play();
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
            if (IsSliding)
                return;

            if (WouldHitWall(dir * 16))
                return;

            IsSliding = true;

            _velocity = dir.Normalized() * SlideSpeed;
            _collider.Disabled = true;
            _wallDetector.Enabled = true;

            _slideStartSound.Play();
            _slideLoopSound.Play();
        }

        private void OnPushedLeft() => Push(Vector2.Left);
        private void OnPushedRight() => Push(Vector2.Right);
        private void OnPushedDown() => Push(Vector2.Down);
        private void OnPushedUp() => Push(Vector2.Up);
    }
}

