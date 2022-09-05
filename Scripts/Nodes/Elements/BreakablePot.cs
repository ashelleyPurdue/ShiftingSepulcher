using Godot;

using RandomDungeons.Nodes.Components;

namespace RandomDungeons.Nodes.Elements
{
    public class BreakablePot : KinematicBody2D
    {
        private const float Friction = 500;
        private const float MinSpeedForCollisionDamage = 90;
        private const float DeathAnimationDuration = 0.1f;

        [Export] public int Health = 2;

        private Vector2 _velocity = Vector2.Zero;
        private float _invulnerableTimer = 0;

        public override void _PhysicsProcess(float delta)
        {
            if (Health > 0)
                WhileAlive(delta);
            else
                WhileDead(delta);
        }

        private void WhileAlive(float delta)
        {
            _invulnerableTimer -= delta;

            // Move
            var prevVel = _velocity;
            _velocity = MoveAndSlide(_velocity);
            _velocity = _velocity.MoveToward(Vector2.Zero, Friction * delta);

            // Take damage when hitting a wall
            bool wasGoingFastEnough = prevVel.Length() > MinSpeedForCollisionDamage;

            if (GetSlideCount() > 0 && wasGoingFastEnough)
            {
                Health--;
                return;
            }
        }

        private void WhileDead(float delta)
        {
            // Play the death animation
            float animSpeed = delta / DeathAnimationDuration;
            Scale = Scale.MoveToward(Vector2.One * 2, animSpeed);

            var color = Modulate;
            color.a -= animSpeed;

            if (color.a <= 0)
            {
                color.a = 0;
                QueueFree();
            }

            Modulate = color;
        }

        public void OnTookDamage(HitBox hitBox)
        {
            // Invulnerability only protects you from damage.
            // It doesn't protect you from knockback.
            _velocity = hitBox.KnockbackVelocity;

            if (_invulnerableTimer > 0)
                return;

            Health -= hitBox.Damage;
            _invulnerableTimer = hitBox.InvlunerabilityTime;
        }
    }
}
