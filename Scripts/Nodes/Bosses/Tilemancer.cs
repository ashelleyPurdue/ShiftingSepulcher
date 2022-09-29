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
            _tilesToThrow
                .Dequeue()
                .Throw(TileThrowSpeed);
        }
    }
}
