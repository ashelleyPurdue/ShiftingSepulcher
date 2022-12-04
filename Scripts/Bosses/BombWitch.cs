using System;
using Godot;

namespace RandomDungeons
{
    public class BombWitch : Node
    {
        [Export] public PackedScene SpawningSpellPrefab;
        [Export] public PackedScene BombPrefab;

        [Export] public float TeleportRadius = 6 * 32;
        [Export] public bool RotateTowardPlayer = false;

        private AnimationPlayer _attackPatterns => GetNode<AnimationPlayer>("%AttackPatterns");
        private AnimationPlayer _animator => GetNode<AnimationPlayer>("%AnimationPlayer");
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

        public void OnShieldShattered(Area2D area)
        {
            _attackPatterns.PlayAndAdvance("DazedLoop");
        }

        public void RecoverFromDazed()
        {
            _attackPatterns.PlayAndAdvance("MainCycle");
        }

        public void CastQueuedSpell()
        {
            _queuedSpell?.Invoke();
        }

        public void QueueBombSpell()
        {
            _queuedSpell = CastBombSpell;
        }

        private void CastBombSpell()
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
