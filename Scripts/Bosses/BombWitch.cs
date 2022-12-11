using System;
using Godot;

namespace RandomDungeons
{
    public class BombWitch : Node
    {
        [Export] public PackedScene SpawningSpellPrefab;
        [Export] public PackedScene BombPrefab;
        [Export] public PackedScene FireballPrefab;

        [Export] public float TeleportRadius = 6 * 32;
        [Export] public float FireballSpeed = 6 * 32;
        [Export] public bool RotateTowardPlayer = false;

        private AnimationPlayer _attackPatterns => GetNode<AnimationPlayer>("%AttackPatterns");
        private AnimationPlayer _animator => GetNode<AnimationPlayer>("%AnimationPlayer");
        private AnimationPlayer _shieldAnimator => GetNode<AnimationPlayer>("%ShieldAnimator");

        private HurtBox _hurtBox => GetNode<HurtBox>("%HurtBox");
        private HurtBox _shieldHurtBox => GetNode<HurtBox>("%ShieldHurtBox");

        private Node2D _spellSpawnPos => GetNode<Node2D>("%SpellSpawnPos");
        private Node2D _body => GetParent<Node2D>();

        private Action _queuedSpell;

        public override void _PhysicsProcess(float delta)
        {
            // Rotate toward the player while aiming
            if (RotateTowardPlayer)
            {
                Vector2 playerPos = PlayerGlobalPos();

                float targetRotRad = _body
                    .GlobalPosition
                    .AngleToPoint(playerPos);

                targetRotRad -= Mathf.Deg2Rad(90 + 180);

                _body.Rotation = Mathf.LerpAngle(
                    _body.Rotation,
                    targetRotRad,
                    0.125f
                );
            }
        }

        public void StartTeleport()
        {
            _animator.ResetAndPlay("Teleport");
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

            _body.Position = new Vector2(
                radius * Mathf.Cos(angle),
                radius * Mathf.Sin(angle)
            );
        }

        public void OnTookDamage(HitBox hitbox)
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
            var bomb = BombPrefab.Instance<Bomb>();
            bomb.LightFuse();   // The timer doesn't actually start ticking down
                                // until the spell reaches its destination and
                                // adds the bomb to the scene tree

            var spawningSpell = SpawningSpellPrefab.Instance<SpawningSpell>();
            _body.GetParent().AddChild(spawningSpell);

            spawningSpell.NodeToSpawn = bomb;
            spawningSpell.TargetPosGlobal = PlayerGlobalPos();
            spawningSpell.GlobalPosition = _spellSpawnPos.GlobalPosition;
        }

        private void ExecuteFireballSpell()
        {
            var fireball = FireballPrefab.Instance<Fireball>();
            fireball.Velocity = _body.GlobalPosition.DirectionTo(PlayerGlobalPos());
            fireball.Velocity *= FireballSpeed;

            _body.GetParent().AddChild(fireball);
            fireball.GlobalPosition = _spellSpawnPos.GlobalPosition;

            // Make the fireballs ignore our own hurtboxes
            foreach (var hitbox in fireball.AllDescendantsOfType<HitBox>())
            {
                hitbox.IgnoreHurtBox(_hurtBox);
                hitbox.IgnoreHurtBox(_shieldHurtBox);
            }
        }

        private Vector2 PlayerGlobalPos()
        {
            var player = GetTree().FindPlayer();

            // If there is no player in this scene, just pretend that the player
            // is in the center of the room.
            // This way, the script does crash inside debug scenes where the
            // player doesn't exist
            if (player == null)
                return _body.GetParent<Node2D>().GlobalPosition;

            return player.GlobalPosition;
        }
    }
}
