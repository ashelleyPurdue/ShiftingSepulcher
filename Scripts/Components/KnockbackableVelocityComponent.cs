using Godot;

namespace RandomDungeons
{
    [CustomNode]
    public class KnockbackableVelocityComponent : BaseComponent<KinematicBody2D>
    {
        [Signal] public delegate void HitWall();
        [Export] public float KnockbackFriction = 500;
        [Export] public float MinSpeedForHitWallTrigger = 90;

        public Vector2 WalkVelocity;

        private Vector2 _velocity;
        private float _knockbackTimer = 0;

        public override void _PhysicsProcess(float delta)
        {
            bool isKnockedBack = _knockbackTimer > 0;

            if (!isKnockedBack)
            {
                _velocity = WalkVelocity;
                _velocity = Entity.MoveAndSlide(_velocity);
            }
            else
            {
                _knockbackTimer -= delta;

                var prevVel = _velocity;
                _velocity = _velocity.MoveToward(WalkVelocity, KnockbackFriction * delta);
                _velocity = Entity.MoveAndSlide(_velocity);

                // Take damage upon hitting a wall too hard
                bool hitWall = Entity.GetSlideCount() > 0;
                bool fastEnough = prevVel.Length() > MinSpeedForHitWallTrigger;
                if (hitWall && fastEnough)
                {
                    EmitSignal(nameof(HitWall));
                }
            }
        }

        public void ApplyKnockback(HitBox damageSource)
        {
            _velocity = damageSource.GetKnockbackVelocity(Entity, KnockbackFriction);
            _knockbackTimer = KnockbackDuration(_velocity);
        }

        private float KnockbackDuration(Vector2 velocity)
        {
            return velocity.Length() / KnockbackFriction;
        }
    }
}
