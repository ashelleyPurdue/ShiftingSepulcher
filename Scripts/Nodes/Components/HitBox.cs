using Godot;

namespace RandomDungeons.Nodes.Components
{
    public class HitBox : Area2D
    {
        [Signal] public delegate void DealtDamage(HurtBox hurtBox);
        [Export] public Vector2 KnockbackVelocity;

        public override void _PhysicsProcess(float delta)
        {
            if (Monitoring)
            {
                foreach (var other in GetOverlappingAreas())
                {
                    if (other is HurtBox hurtBox)
                    {
                        hurtBox.TakeDamage(this);
                        EmitSignal(nameof(DealtDamage), hurtBox);
                    }
                }
            }
        }
    }
}
