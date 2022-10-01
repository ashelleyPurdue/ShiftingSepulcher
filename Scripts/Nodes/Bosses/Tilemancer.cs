using System;
using System.Collections.Generic;
using Godot;

using RandomDungeons.Utils;
using RandomDungeons.Nodes.Elements.Projectiles;

namespace RandomDungeons.Nodes.Bosses
{
    public class Tilemancer : Node2D
    {
        [Export] public PackedScene TilePrefab;
        [Export] public float ArenaHeight = 32 * 16;
        [Export] public float ArenaWidth = 32 * 16;
        [Export] public float TileThrowSpeed = 32 * 19;
        [Export] public float JumpProgress;

        private Node2D _player;
        private Vector2 _jumpStartPos;

        private Queue<TilemancerTile> _tilesToThrow = new Queue<TilemancerTile>();

        public override void _Ready()
        {
            _player = GetTree().FindPlayer();
        }

        public override void _PhysicsProcess(float delta)
        {
            if (JumpProgress < 1)
            {
                GlobalPosition = _jumpStartPos.LinearInterpolate(
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
    }
}
