using Godot;

namespace RandomDungeons
{
    public class BreakablePot : KinematicBody2D
    {
        [Export] public float ThrowDistance = 32 * 5;
        [Export] public float ThrowFlyTime = 1;

        private AnimationPlayer _animator => GetNode<AnimationPlayer>("%AnimationPlayer");
        private bool _isDead = false;
        private bool _isFlying = false;
        private Vector2 _velocity;

        public override void _PhysicsProcess(float delta)
        {
            CollisionLayer = _isFlying
                ? (uint)CollisionLayerBits.StopsEnemiesOnly
                : (uint)CollisionLayerBits.Walls;

            if (!_isFlying)
                return;

            var collision = MoveAndCollide(_velocity * delta);
            if (collision != null)
            {
                Shatter();
            }
        }

        public void OnThrown(Vector2 direction)
        {
            _isFlying = true;
            _velocity = direction * (ThrowDistance / ThrowFlyTime);

            GetTree()
                .CreateTimer(ThrowFlyTime)
                .Connect("timeout", this, nameof(Shatter));
        }

        public void OnTookDamage(HitBox hitBox)
        {
            if (!_isDead)
                Shatter();
        }

        private void Shatter()
        {
            _isDead = true;
            _isFlying = false;
            _animator.Play("Shatter");
        }
    }
}
