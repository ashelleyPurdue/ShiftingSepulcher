using Godot;

namespace RandomDungeons
{
    public class Fireball : KinematicBody2D
    {
        [Export] public float LifeSpanSeconds = 10;
        public Vector2 Velocity;

        private AnimationPlayer _animator => GetNode<AnimationPlayer>("%AnimationPlayer");

        private float _timer = 0;
        private bool _isDead = false;

        public override void _PhysicsProcess(float delta)
        {
            if (_isDead)
                return;

            var collision = MoveAndCollide(Velocity * delta);
            if (collision != null)
            {
                Die();
                return;
            }

            _timer += delta;
            if (_timer >= LifeSpanSeconds)
            {
                Die();
                return;
            }
        }

        public void Die()
        {
            _isDead = true;
            _animator.ResetAndPlay("Die");
        }
    }
}
