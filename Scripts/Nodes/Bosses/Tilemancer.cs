using System;
using System.Collections.Generic;
using Godot;

using RandomDungeons.Utils;
using RandomDungeons.Nodes.Elements.Projectiles;
using RandomDungeons.Nodes.Components;

namespace RandomDungeons.Nodes.Bosses
{
    public class Tilemancer : Node2D
    {
        [Export] public PackedScene VictoryChestPrefab;
        [Export] public PackedScene TilePrefab;
        [Export] public int Health = 9;
        [Export] public float ArenaHeight = 32 * 16;
        [Export] public float ArenaWidth = 32 * 16;
        [Export] public float TileThrowSpeed = 32 * 19;
        [Export] public float JumpProgress;

        private Node2D _player;
        private Vector2 _jumpStartPos;
        private HurtFlasher _hurtFlasher => GetNode<HurtFlasher>("%HurtFlasher");
        private AnimationPlayer _animationPlayer => GetNode<AnimationPlayer>("%AnimationPlayer");

        private Queue<TilemancerTile> _tilesToThrow = new Queue<TilemancerTile>();
        private bool _isDead = false;

        public override void _Ready()
        {
            _player = GetTree().FindPlayer();
            _jumpStartPos = GlobalPosition;
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

        public void StartAttackLoop()
        {
            _animationPlayer.CurrentAnimation = "AttackLoop";
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

        public void DestoryAllTiles()
        {
            while (_tilesToThrow.Count > 0)
            {
                var tile = _tilesToThrow.Dequeue();

                if (IsInstanceValid(tile))
                    tile.Shatter();
            }
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
            _animationPlayer.CurrentAnimation = "Death";
        }
    }
}
