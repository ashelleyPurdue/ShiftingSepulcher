using Godot;

namespace RandomDungeons
{
    public class BreakablePot : KinematicBody2D
    {
        [Export] public float ThrowDistance = 32 * 5;
        [Export] public float ThrowSpeed = 32 * 20;

        private AnimationPlayer _animator => GetNode<AnimationPlayer>("%AnimationPlayer");
        private HitBox _hitBox => GetNode<HitBox>("%HitBox");

        private bool _isDead = false;
        private bool _isFlying = false;
        private Vector2 _velocity;

        public override void _PhysicsProcess(float delta)
        {
            CollisionLayer = _isFlying
                ? (uint)CollisionLayerBits.StopsEnemiesOnly
                : (uint)CollisionLayerBits.Walls;

            _hitBox.Monitoring = _isFlying;

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
            _velocity = direction * ThrowSpeed;

            GetTree()
                .CreateTimer(ThrowDistance / ThrowSpeed)
                .Connect("timeout", this, nameof(Shatter));
        }

        public void OnTookDamage(HitBox hitBox)
        {
            Shatter();
        }

        public void OnDealtDamage(HurtBox hurtBox)
        {
            Shatter();
        }

        private void Shatter()
        {
            if (_isDead)
                return;

            _isDead = true;
            _isFlying = false;
            _animator.Play("Shatter");
        }
    }
}
