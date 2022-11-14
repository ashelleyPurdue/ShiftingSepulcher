using Godot;

namespace RandomDungeons
{
    public class BreakablePot : KinematicBody2D
    {
        [Export] public float ThrowDistance = 32 * 5;
        [Export] public float ThrowSpeed = 32 * 20;

        private AnimationPlayer _animator => GetNode<AnimationPlayer>("%AnimationPlayer");

        private bool _isDead = false;
        private bool _isFlying = false;
        private Vector2 _velocity;

        public override void _PhysicsProcess(float delta)
        {
            // Don't shatter when hitting the player, since the player is the
            // one who threw it.
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
            _velocity = direction * ThrowSpeed;

            float throwDuration = ThrowDistance / ThrowSpeed;
            _animator.PlaybackSpeed = 1 / throwDuration;
            _animator.Play("Throw");
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
            _animator.PlaybackSpeed = 1;
            _animator.Play("Shatter");
        }
    }
}
