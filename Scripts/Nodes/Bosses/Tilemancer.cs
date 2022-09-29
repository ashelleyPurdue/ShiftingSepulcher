using Godot;

using RandomDungeons.Nodes.Elements.Enemies;

namespace RandomDungeons.Nodes.Bosses
{
    public class Tilemancer : AnimationPlayer
    {
        public void SummonTile()
        {
            GD.Print("Fwip!");
        }

        public void ThrowTile()
        {
            GD.Print("Whoosh!");
        }
    }
}
