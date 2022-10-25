using Godot;

namespace RandomDungeons
{
    public class EnemyBody : KinematicBody2D, IRespawnable, IEnemy
    {
        [Signal] public delegate void HitWall();
        [Signal] public delegate void Dead();
        [Signal] public delegate void Respawning();

        [Export] public int MaxHealth = 1;
        [Export] public float KnockbackFriction = 500;
        [Export] public float MinSpeedForHitWallTrigger = 90;

        public bool IsDead {get; private set;}

        public int Health;
        public Vector2 WalkVelocity;
        public Vector2 KnockbackVelocity;

        private HurtFlasher _hurtFlasher => GetNode<HurtFlasher>("%HurtFlasher");

        private bool _spawnPosKnown = false;
        private Vector2 _spawnPos;

        public override void _Ready()
        {
            _spawnPos = Position;
            _spawnPosKnown = true;

            Respawn();
        }

        public void Respawn()
        {
            Health = MaxHealth;
            IsDead = false;

            if (_spawnPosKnown)
                Position = _spawnPos;

            EmitSignal(nameof(Respawning));
        }

        public override void _PhysicsProcess(float delta)
        {
            // Move
            Vector2 prevKnockbackVel = KnockbackVelocity;
            Vector2 totalVelocity = WalkVelocity + KnockbackVelocity;
            totalVelocity = MoveAndSlide(totalVelocity);
            KnockbackVelocity = totalVelocity - WalkVelocity;
            KnockbackVelocity = KnockbackVelocity.MoveToward(
                Vector2.Zero,
                KnockbackFriction * delta
            );

            // Take damage upon hitting a wall too hard
            bool hitWall = GetSlideCount() > 0;
            bool fastEnough = prevKnockbackVel.Length() > MinSpeedForHitWallTrigger;
            if (hitWall && fastEnough)
            {
                Health--;
                _hurtFlasher.Flash();
                EmitSignal(nameof(HitWall));
            }

            // Die when out of health
            if (Health <= 0 && !IsDead)
            {
                _hurtFlasher?.Cancel();
                IsDead = true;
                EmitSignal(nameof(Dead));
            }
        }

        public virtual void OnTookDamage(HitBox hitBox)
        {
            Health -= hitBox.Damage;
            KnockbackVelocity = hitBox.GetKnockbackVelocity(this, KnockbackFriction);
            _hurtFlasher?.Flash();
        }
    }
}
