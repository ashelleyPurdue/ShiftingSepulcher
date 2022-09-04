using Godot;

namespace RandomDungeons.Nodes.Components
{
    public class HurtBox : Area2D
    {
        [Signal] public delegate void TookDamage(HitBox hitBox);

        public void TakeDamage(HitBox hitBox)
        {
            EmitSignal(nameof(TookDamage), hitBox);
        }
    }
}