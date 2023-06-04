using System;
using Godot;

namespace ShiftingSepulcher
{
    [CustomNode]
    public class EnemyComponent : BaseComponent<Node2D>, IRespawnable, IEnemy, IOnRoomEnter
    {
        [Signal] public delegate void Dying();
        [Signal] public delegate void Dead();
        [Signal] public delegate void Respawning();

        /// <summary>
        /// If this is true, the enemy will stay dead when they're killed, even
        /// after the player dies.
        /// </summary>
        /// <value></value>
        [Export] public bool DiesPermanently = false;
        [Export] public bool DisableLootDrops = false;

        /// <summary>
        /// If this is true, <see cref="Dead"/> will be fired immediately after
        /// <see cref="Dying"/>, without waiting for
        /// <see cref="FireDeathAnimationComplete"/>.
        ///
        /// Use this for enemies whose death animation has not been implemented
        /// yet.
        ///
        /// If this is false, then the entity must call
        /// <see cref="FireDeathAnimationComplete"/> when the death animation
        /// completes, or else <see cref="Dead"/> will never fire.
        /// </summary>
        /// <value></value>
        [Export] public bool SkipDeathAnimation = true;

        public bool IsAlive {get; private set;} = true;

        private HealthPointsComponent _healthPoints => this.GetComponent<HealthPointsComponent>();

        private bool _spawnPosKnown = false;
        private Vector2 _spawnPos;

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

            _spawnPos = Entity.Position;
            _spawnPosKnown = true;

            Respawn();
        }

        public void OnRoomEnter()
        {
            if (IsAlive)
                Respawn();
        }

        public void Respawn()
        {
            if (!IsAlive && DiesPermanently)
                return;

            _healthPoints.Health = _healthPoints.MaxHealth;
            IsAlive = true;

            if (_spawnPosKnown)
                Entity.Position = _spawnPos;

            EmitSignal(nameof(Respawning));
        }

        public void OnTookDamage()
        {
            // Die when out of health
            if (_healthPoints.Health <= 0 && IsAlive)
            {
                IsAlive = false;

                if (!DisableLootDrops)
                {
                    // TODO: Move this into a signal handler for Dead.
                    foreach (var lootDropper in this.GetComponents<ILootDropperComponent>())
                        lootDropper.DropLoot();
                }

                EmitSignal(nameof(Dying));

                if (SkipDeathAnimation)
                    EmitSignal(nameof(Dead));
            }
        }

        public void FireDeathAnimationComplete()
        {
            if (SkipDeathAnimation)
                throw new InvalidOperationException("The death animation was already skipped");

            if (IsAlive)
                throw new InvalidOperationException("This enemy is still alive!");

            // TODO: Throw an error if this is called more than once before
            // respawning.

            EmitSignal(nameof(Dead));
        }

        private void OnHitWall()
        {
            _healthPoints.TakeDamageDisregardingInvulnerability(1);
        }
    }
}
