using System;
using System.Collections.Generic;
using Godot;

namespace RandomDungeons
{
    public class Tilemancer : Node2D, IRespawnable
    {
        [Export] public PackedScene VictoryChestPrefab;
        [Export] public PackedScene TilePrefab;
        [Export] public int MaxHealth = 9;
        [Export] public float ArenaHeight = 32 * 16;
        [Export] public float ArenaWidth = 32 * 16;
        [Export] public float TileThrowSpeed = 32 * 19;
        [Export] public float JumpProgress;

        [Export] public int Health;

        private Player _player;
        private Vector2 _jumpStartPos;
        private HurtFlasher _hurtFlasher => GetNode<HurtFlasher>("%HurtFlasher");
        private AnimationPlayer _mainAnimationPlayer => GetNode<AnimationPlayer>("%MainAnimationPlayer");
        private AnimationPlayer _individualAnimations => GetNode<AnimationPlayer>("%IndividualAnimations");

        private bool _spawnPosKnown = false;
        private Vector2 _spawnPos;
        private Queue<TilemancerTile> _tilesToThrow = new Queue<TilemancerTile>();
        private bool _isDead = false;

        [Signal] private delegate void ShatterAllTilesSignal();
        [Signal] private delegate void DestroyAllTilesSignal();

        public override void _Ready()
        {
            _spawnPos = Position;
            _spawnPosKnown = true;

            _player = GetTree().FindPlayer();

            Respawn();
        }

        public void Respawn()
        {
            if (_isDead)
                return;

            Health = MaxHealth;

            if (_spawnPosKnown)
                Position = _spawnPos;

            _jumpStartPos = GlobalPosition;

            _mainAnimationPlayer.Play("RESET");
            _mainAnimationPlayer.Advance(0);

            _individualAnimations.Play("RESET");
            _individualAnimations.Advance(0);

            _mainAnimationPlayer.Play("Intro");

            DestoryAllTiles();
        }

        public override void _PhysicsProcess(float delta)
        {
            // Home in on the player during the jump animation
            if (JumpProgress < 1)
            {
                GlobalPosition = _jumpStartPos.LinearInterpolate(
                    _player.GlobalPosition,
                    Mathf.SmoothStep(0, 1, JumpProgress)
                );
            }

            // Die when out of health
            if (Health <= 0 && !_isDead)
                Die();
        }

        public void OnTookDamage(HitBox hitBox)
        {
            Health--;
            _hurtFlasher.Flash();
        }

        /// <summary>
        /// Used to freeze the player during the intro animation, so they can't
        /// just wail on the boss while he's doing his evil monologue.
        /// </summary>
        /// <param name="frozen"></param>
        public void SetPlayerFrozen(bool frozen)
        {
            _player.ControlsEnabled = !frozen;
        }

        public void StartAttackLoop()
        {
            _player.ControlsEnabled = true;
            _mainAnimationPlayer.CurrentAnimation = "AttackLoop";
        }

        public void SummonTile()
        {
            Vector2 tilePos = new Vector2(
                (GD.Randf() * ArenaWidth) - (ArenaWidth / 2),
                (GD.Randf() * ArenaHeight) - (ArenaHeight / 2)
            );

            var tile = TilePrefab.Instance<TilemancerTile>();
            _tilesToThrow.Enqueue(tile);

            GetParent().AddChild(tile);
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
            _jumpStartPos = GlobalPosition;
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

        public void SpawnVictoryChest()
        {
            var chest = VictoryChestPrefab.Instance<Node2D>();
            GetParent().AddChild(chest);
            chest.Position = Vector2.Zero;
        }

        private void Die()
        {
            _isDead = true;
            _mainAnimationPlayer.CurrentAnimation = "Death";
        }
    }
}
