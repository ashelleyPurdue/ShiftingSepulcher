using Godot;
using RandomDungeons.Nodes.Components;

namespace RandomDungeons.Nodes.Elements
{
    public class RotatableLaserMirror : Node2D
    {
        [Export] public float DegreesPerHit = 45;

        public void OnTookDamage(HitBox hitBox)
        {
            RotationDegrees += DegreesPerHit;
        }
    }
}
