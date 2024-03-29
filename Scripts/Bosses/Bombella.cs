using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace ShiftingSepulcher
{
    public class Bombella : BaseComponent<Node2D>, IOnRoomTransitionFinished, IChallenge
    {
        [Export] public PackedScene VictoryChestPrefab;
        [Export] public PackedScene SpawningSpellPrefab;
        [Export] public PackedScene BombPrefab;
        [Export] public PackedScene FireballPrefab;

        [Export] public float TeleportRadius = 6 * 32;
        [Export] public float FireballSpeed = 6 * 32;
        [Export] public bool RotateTowardPlayer = false;
        [Export] public float AimProgressPercent = 0;

        private AnimationPlayer _attackPatterns => GetNode<AnimationPlayer>("%AttackPatterns");
        private AnimationPlayer _animator => GetNode<AnimationPlayer>("%AnimationPlayer");
        private AnimationPlayer _shieldAnimator => GetNode<AnimationPlayer>("%ShieldAnimator");

        private HurtBox _shieldHurtBox => GetNode<HurtBox>("%ShieldHurtBox");

        private Node2D _spellSpawnPos => GetNode<Node2D>("%SpellSpawnPos");

        private EnemyComponent _enemy => this.GetComponent<EnemyComponent>();
        private HealthPointsComponent _hp => this.GetComponent<HealthPointsComponent>();

        private Action _queuedSpell;

        private List<Node> _spawnedProjectiles = new List<Node>();

        private float _aimStartRotRad;
        private bool _deathAnimationFinished = false;

        bool IChallenge.IsSolved() => _deathAnimationFinished;

        public override void _PhysicsProcess(float delta)
        {
            // Rotate toward the player while aiming
            if (RotateTowardPlayer)
            {
                Vector2 playerPos = PlayerGlobalPos();

                float targetRotRad = Entity
                    .GlobalPosition
                    .AngleToPoint(playerPos);

                targetRotRad -= Mathf.Deg2Rad(90 + 180);

                Entity.Rotation = Mathf.LerpAngle(
                    _aimStartRotRad,
                    targetRotRad,
                    AimProgressPercent
                );
            }
        }

        public void StartTeleport()
        {
            _animator.ResetAndPlay("Teleport");
        }

        public void StartAiming()
        {
            _aimStartRotRad = Entity.Rotation;
        }

        public void CastBombSpell()
        {
            _queuedSpell = ExecuteBombSpell;
            _animator.ResetAndPlay("Throw");
        }

        public void CastFireballSpell()
        {
            _queuedSpell = ExecuteFireballSpell;
            _animator.ResetAndPlay("Throw");
        }

        public void ResetAnimator()
        {
            _animator.Reset();
        }

        public void TeleportToRandomSpot()
        {
            float angle = GD.Randf() * Mathf.Deg2Rad(360);
            float radius = GD.Randf() * TeleportRadius;

            Entity.Position = new Vector2(
                radius * Mathf.Cos(angle),
                radius * Mathf.Sin(angle)
            );
        }

        public void OnRespawning()
        {
            Entity.RotationDegrees = 0;

            _shieldAnimator.Reset();
            _attackPatterns.Stop(true);

            // Jump to the first frame of the intro animation,
            // but don't start playing it until the room transition finishes
            _animator.ResetAndPlay("Intro");
            _animator.PlaybackSpeed = 0;

            DeleteAllProjectiles();
        }

        public void OnRoomTransitionFinished()
        {
            if (!_enemy.IsAlive)
                return;

            _animator.PlaybackSpeed = 1;
            _animator.ResetAndPlay("Intro");
            GetTree().FindPlayer().FreezeForCutscene();
        }

        public void OnIntroAnimationFinished()
        {
            GetTree().FindPlayer().UnfreezeForCutscene();
            OnFinishedRecoveringFromDaze();
        }

        public void OnTookDamage()
        {
            _attackPatterns.Stop();
            _animator.ResetAndPlay("Ouch");
            _animator.Queue("DazeRecover");
        }

        public void OnShieldTookDamage(HitBox hitbox)
        {
            if (hitbox.IsAncestorInGroup("Explosion"))
            {
                _attackPatterns.PlayAndAdvance("DazedLoop");
                _shieldAnimator.ResetAndPlay("Shatter");
                return;
            }

            _shieldAnimator.ResetAndPlay("Pulse");
        }

        public void OnFinishedRecoveringFromDaze()
        {
            _attackPatterns.PlayAndAdvance("MainCycle");
            _shieldAnimator.ResetAndPlay("Reform");
        }

        public void OnDead()
        {
            _attackPatterns.Stop();
            _animator.ResetAndPlay("Death");
        }

        public void OnDeathAnimationFinished()
        {
            _deathAnimationFinished = true;

            var chest = VictoryChestPrefab.Instance<Node2D>();
            this.GetRoom().AddChild(chest);
        }

        public void RecoverFromDazed()
        {
            _attackPatterns.Stop();
            _animator.ResetAndPlay("DazeRecover");
        }

        public void ExecuteQueuedSpell()
        {
            _queuedSpell?.Invoke();
        }

        private void ExecuteBombSpell()
        {
            var bomb = SpawnProjectile<Bomb>(BombPrefab);
            bomb.LightFuse();   // The timer doesn't actually start ticking down
                                // until the spell reaches its destination and
                                // adds the bomb to the scene tree

            var spawningSpell = SpawnProjectile<SpawningSpell>(SpawningSpellPrefab);
            this.GetRoom().AddChild(spawningSpell);

            spawningSpell.NodeToSpawn = bomb;
            spawningSpell.TargetPosGlobal = PlayerGlobalPos();
            spawningSpell.GlobalPosition = _spellSpawnPos.GlobalPosition;
        }

        private void ExecuteFireballSpell()
        {
            var fireball = SpawnProjectile<Fireball>(FireballPrefab);
            this.GetRoom().AddChild(fireball);
            fireball.GlobalPosition = _spellSpawnPos.GlobalPosition;

            fireball.Velocity = Entity.GlobalPosition.DirectionTo(PlayerGlobalPos());
            fireball.Velocity *= FireballSpeed;

            // Make the fireballs ignore our own hurtboxes
            foreach (var hitbox in fireball.AllDescendantsOfType<HitBox>())
            {
                hitbox.Ignore(_hp);
                hitbox.IgnoreHurtBox(_shieldHurtBox);
            }
        }

        /// <summary>
        /// Spawns an instance of the given prefab and keeps track of it, such
        /// that we can delete all created projectiles later.
        ///
        /// This does NOT add the node to the scene tree, or does it set its
        /// position.
        /// </summary>
        /// <param name="prefab"></param>
        /// <typeparam name="TNode"></typeparam>
        /// <returns></returns>
        private TNode SpawnProjectile<TNode>(PackedScene prefab)
            where TNode : Node2D
        {
            ForgetDeletedProjectiles();

            var projectile = prefab.Instance<TNode>();
            _spawnedProjectiles.Add(projectile);

            return projectile;
        }

        private void DeleteAllProjectiles()
        {
            foreach (var n in _spawnedProjectiles)
            {
                if (IsInstanceValid(n))
                    n.QueueFree();
            }

            ForgetDeletedProjectiles();
        }

        private void ForgetDeletedProjectiles()
        {
            var deadRefs = _spawnedProjectiles
                .Where(n => !IsInstanceValid(n) || n.IsQueuedForDeletion())
                .ToArray();

            foreach (Node n in deadRefs)
                _spawnedProjectiles.Remove(n);
        }

        private Vector2 PlayerGlobalPos()
        {
            var player = GetTree().FindPlayer();

            // If there is no player in this scene, just pretend that the player
            // is in the center of the room.
            // This way, the script does crash inside debug scenes where the
            // player doesn't exist
            if (player == null)
                return this.GetRoom().GlobalPosition;

            return player.GlobalPosition;
        }
    }
}
