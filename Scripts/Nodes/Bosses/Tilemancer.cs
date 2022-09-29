using Godot;

using RandomDungeons.Utils;
using RandomDungeons.Nodes.Elements.Enemies;

namespace RandomDungeons.Nodes.Bosses
{
    public class Tilemancer : AnimationPlayer
    {
        [Export] public PackedScene TilePrefab;
        [Export] public float ArenaHeight = 32 * 16;
        [Export] public float ArenaWidth = 32 * 16;

        public void SummonTile()
        {
            Vector2 tilePos = new Vector2(
                (GD.Randf() * ArenaWidth) - (ArenaWidth / 2),
                (GD.Randf() * ArenaHeight) - (ArenaHeight / 2)
            );

            var tile = TilePrefab.Instance<AnimatedTile>();
            GetParent().AddChild(tile);
            tile.Position = tilePos;
            tile.Target = GetTree().FindPlayer();
            tile.HoldTime = 1.6f;
        }

        public void ThrowTile()
        {
            GD.Print("Whoosh!");
        }
    }
}
