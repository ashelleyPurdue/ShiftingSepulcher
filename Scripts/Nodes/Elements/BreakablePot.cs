using Godot;

using RandomDungeons.Nodes.Components;

namespace RandomDungeons.Nodes.Elements
{
    public class BreakablePot : KinematicBody2D
    {
        private const float Friction = 500;
        private const float MinSpeedForCollisionDamage = 90;

        private Vector2 _velocity = Vector2.Zero;

        public override void _PhysicsProcess(float delta)
        {
            var prevVel = _velocity;
            _velocity = MoveAndSlide(_velocity);

            // Take damage when hitting a wall
            bool wasGoingFastEnough = prevVel.Length() > MinSpeedForCollisionDamage;

            if (GetSlideCount() > 0 && wasGoingFastEnough)
            {
                QueueFree();
                return;
            }

            // Apply friction
            _velocity = _velocity.MoveToward(Vector2.Zero, Friction * delta);
        }

        public void OnTookDamage(HitBox hitBox)
        {
            _velocity = hitBox.KnockbackVelocity;
        }
    }
}
