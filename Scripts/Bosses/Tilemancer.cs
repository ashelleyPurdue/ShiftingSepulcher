using System;
using System.Collections.Generic;
using Godot;

namespace ShiftingSepulcher
{
    public class Tilemancer : BaseComponent<Node2D>, IOnRoomTransitionFinished, IChallenge
    {
        [Export] public PackedScene VictoryChestPrefab;
        [Export] public PackedScene TilePrefab;
        [Export] public float ArenaHeight = 32 * 16;
        [Export] public float ArenaWidth = 32 * 16;
        [Export] public float TileThrowSpeed = 32 * 19;
        [Export] public float JumpProgress;

        private Player _player;
        private Vector2 _jumpStartPos;
        private AnimationPlayer _mainAnimationPlayer => GetNode<AnimationPlayer>("%MainAnimationPlayer");
        private AnimationPlayer _individualAnimations => GetNode<AnimationPlayer>("%IndividualAnimations");

        private EnemyComponent _enemy => this.GetComponent<EnemyComponent>();
        private HealthPointsComponent _hp => this.GetComponent<HealthPointsComponent>();

        private Queue<TilemancerTile> _tilesToThrow = new Queue<TilemancerTile>();
        private bool _deathAnimationFinished = false;

        [Signal] private delegate void ShatterAllTilesSignal();
        [Signal] private delegate void DestroyAllTilesSignal();

        bool IChallenge.IsSolved() => _deathAnimationFinished;

        public override void _EntityReady()
        {
            _player = GetTree().FindPlayer();
            Respawn();
        }

        public void Respawn()
        {
            _jumpStartPos = Entity.GlobalPosition;

            _mainAnimationPlayer.Reset();
            _individualAnimations.Reset();

            // Jump to the first frame of the intro animation, but don't actually
            // _play_ the animation until after the room transition finishes.
            _mainAnimationPlayer.PlayAndAdvance("Intro");
            _mainAnimationPlayer.PlaybackSpeed = 0;

            DestoryAllTiles();
        }

        public void OnRoomTransitionFinished()
        {
            if (!_enemy.IsDead)
            {
                _mainAnimationPlayer.PlaybackSpeed = 1;
                _mainAnimationPlayer.ResetAndPlay("Intro");
                _player.FreezeForCutscene();
            }
        }

        public void OnIntroAnimationFinished()
        {
            _player.UnfreezeForCutscene();
            _mainAnimationPlayer.ResetAndPlay("AttackLoop");
        }

        public override void _PhysicsProcess(float delta)
        {
            // Home in on the player during the jump animation
            if (JumpProgress < 1)
            {
                Entity.GlobalPosition = _jumpStartPos.LinearInterpolate(
                    _player.GlobalPosition,
                    Mathf.SmoothStep(0, 1, JumpProgress)
                );
            }
        }

        public void SummonTile()
        {
            Vector2 tilePos = new Vector2(
                (GD.Randf() * ArenaWidth) - (ArenaWidth / 2),
                (GD.Randf() * ArenaHeight) - (ArenaHeight / 2)
            );

            var tile = TilePrefab.Instance<TilemancerTile>();
            _tilesToThrow.Enqueue(tile);

            this.GetRoom().AddChild(tile);
            tile.Position = tilePos;

            // Set it up so we can remotely detonate this tile without holding
            // a reference to it.
            Connect(
                signal: nameof(ShatterAllTilesSignal),
                target: tile,
                method: nameof(tile.Shatter),
                flags: (int)(ConnectFlags.ReferenceCounted | ConnectFlags.Oneshot)
            );
            Connect(
                signal: nameof(DestroyAllTilesSignal),
                target: tile,
                method: "queue_free",
                flags: (int)(ConnectFlags.ReferenceCounted | ConnectFlags.Oneshot)
            );

            // Make sure we can't be hurt by our own tiles
            foreach (var hitbox in tile.AllDescendantsOfType<HitBox>())
            {
                hitbox.Ignore(_hp);
            }
        }

        public void ThrowTile()
        {
            var tile = _tilesToThrow.Dequeue();

            // The tile may have been destroyed before we had time to throw it.
            // When that happens, a dangling pointer will be left in the queue.
            //
            // Yes.  A dangling pointer.  In C#.
            // That's apparently a thing in Godot.
            if (!IsInstanceValid(tile))
                return;

            tile.Throw(TileThrowSpeed);
        }

        public void TargetPlayerForJump()
        {
            _jumpStartPos = Entity.GlobalPosition;
        }

        private void DestoryAllTiles()
        {
            EmitSignal(nameof(DestroyAllTilesSignal));
            _tilesToThrow.Clear();
        }

        private void ShatterAllTiles()
        {
            EmitSignal(nameof(ShatterAllTilesSignal));
            _tilesToThrow.Clear();
        }

        public void OnDeathAnimationFinished()
        {
            _deathAnimationFinished = true;

            var chest = VictoryChestPrefab.Instance<Node2D>();
            this.GetRoom().AddChild(chest);
            chest.Position = Vector2.Zero;
        }

        private void Die()
        {
            _mainAnimationPlayer.ResetAndPlay("Death");
            ShatterAllTiles();
        }
    }
}
