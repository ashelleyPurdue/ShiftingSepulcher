using Godot;

namespace RandomDungeons
{
    [CustomNode]
    public class EnemyComponent : BaseComponent<Node2D>, IRespawnable, IEnemy, IOnRoomEnter
    {
        [Signal] public delegate void Dead();
        [Signal] public delegate void Respawning();

        /// <summary>
        /// If this is true, the enemy will stay dead when they're killed, even
        /// after the player dies.
        /// </summary>
        /// <value></value>
        [Export] public bool DiesPermanently = false;

        public bool IsDead {get; private set;}

        private HealthPointsComponent _healthPoints => this.GetComponent<HealthPointsComponent>();

        private bool _spawnPosKnown = false;
        private Vector2 _spawnPos;

        public override void _Ready()
        {
            _spawnPos = Entity.Position;
            _spawnPosKnown = true;

            Respawn();
        }

        public override void _EntityReady()
        {
            _healthPoints.Connect(
                signal: nameof(HealthPointsComponent.TookDamageNoParams),
                target: this,
                method: nameof(OnTookDamage),
                flags: (uint)ConnectFlags.Deferred
            );

            if (this.HasComponent<KnockbackableVelocityComponent>(out var kb))
            {
                kb.Connect(
                    signal: nameof(KnockbackableVelocityComponent.HitWall),
                    target: this,
                    method: nameof(OnHitWall)
                );

                _healthPoints.Connect(
                    signal: nameof(HealthPointsComponent.TookDamageFromHitBox),
                    target: kb,
                    method: nameof(kb.ApplyKnockback)
                );
            }
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

            _healthPoints.Health = _healthPoints.MaxHealth;
            IsDead = false;

            if (_spawnPosKnown)
                Entity.Position = _spawnPos;

            EmitSignal(nameof(Respawning));
        }

        public void OnTookDamage()
        {
            // Die when out of health
            if (_healthPoints.Health <= 0 && !IsDead)
            {
                IsDead = true;
                EmitSignal(nameof(Dead));
            }
        }

        private void OnHitWall()
        {
            _healthPoints.TakeDamage(1);
        }
    }
}
