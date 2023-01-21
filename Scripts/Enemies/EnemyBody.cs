using Godot;

namespace RandomDungeons
{
    public class EnemyBody : KinematicBody2D, IRespawnable, IEnemy, IOnRoomEnter
    {
        [Signal] public delegate void HitWall();
        [Signal] public delegate void Dead();
        [Signal] public delegate void Respawning();

        [Export] public int MaxHealth = 1;
        [Export] public float KnockbackFriction = 500;
        [Export] public float MinSpeedForHitWallTrigger = 90;

        /// <summary>
        /// If this is true, the enemy will stay dead when they're killed, even
        /// after the player dies.
        /// </summary>
        /// <value></value>
        [Export] public bool DiesPermanently = false;

        public bool IsDead {get; private set;}

        [Export] public int Health;
        public Vector2 WalkVelocity;

        private HurtFlasher _hurtFlasher => GetNode<HurtFlasher>("%HurtFlasher");

        private bool _spawnPosKnown = false;
        private Vector2 _spawnPos;

        private float _knockbackTimer = 0;
        private Vector2 _velocity;

        public override void _Ready()
        {
            _spawnPos = Position;
            _spawnPosKnown = true;

            Respawn();
        }

        public void OnRoomEnter()
        {
            if (!IsDead)
                Respawn();
        }

        public void Respawn()
        {
            if (IsDead && DiesPermanently)
                return;

            Health = MaxHealth;
            IsDead = false;

            if (_spawnPosKnown)
                Position = _spawnPos;

            EmitSignal(nameof(Respawning));
        }

        public override void _PhysicsProcess(float delta)
        {
            bool isKnockedBack = _knockbackTimer > 0;

            if (!isKnockedBack)
            {
                _velocity = WalkVelocity;
                _velocity = MoveAndSlide(_velocity);
            }
            else
            {
                _knockbackTimer -= delta;

                var prevVel = _velocity;
                _velocity = _velocity.MoveToward(WalkVelocity, KnockbackFriction * delta);
                _velocity = MoveAndSlide(_velocity);

                // Take damage upon hitting a wall too hard
                bool hitWall = GetSlideCount() > 0;
                bool fastEnough = prevVel.Length() > MinSpeedForHitWallTrigger;
                if (hitWall && fastEnough)
                {
                    Health--;
                    _hurtFlasher.Flash();
                    EmitSignal(nameof(HitWall));
                }
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
            _hurtFlasher?.Flash();

            _velocity = hitBox.GetKnockbackVelocity(this, KnockbackFriction);
            _knockbackTimer = KnockbackDuration(_velocity);
        }

        private float KnockbackDuration(Vector2 velocity)
        {
            return velocity.Length() / KnockbackFriction;
        }
    }
}
