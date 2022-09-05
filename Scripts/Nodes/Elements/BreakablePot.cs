using Godot;

using RandomDungeons.Nodes.Components;
namespace RandomDungeons.Nodes.Elements
{
    public class BreakablePot : Node2D
    {
        public void OnTookDamage(HitBox hitBox)
        {
            QueueFree();
        }
    }
}
