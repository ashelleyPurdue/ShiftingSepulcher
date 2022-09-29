using System;
using System.Collections.Generic;
using Godot;

using RandomDungeons.Utils;
using RandomDungeons.Nodes.Elements.Projectiles;

namespace RandomDungeons.Nodes.Bosses
{
    public class Tilemancer : AnimationPlayer
    {
        [Export] public PackedScene TilePrefab;
        [Export] public float ArenaHeight = 32 * 16;
        [Export] public float ArenaWidth = 32 * 16;
        [Export] public float TileThrowSpeed = 32 * 19;

        private Queue<TilemancerTile> _tilesToThrow = new Queue<TilemancerTile>();

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
    }
}
