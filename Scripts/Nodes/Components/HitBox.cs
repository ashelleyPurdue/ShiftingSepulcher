using Godot;

namespace RandomDungeons.Nodes.Components
{
    public class HitBox : Area2D
    {
        [Signal] public delegate void DealtDamage(HurtBox hurtBox);
        [Export] public int Damage = 1;
        [Export] public float InvlunerabilityTime = 1;
        [Export] public Vector2 KnockbackVelocity = Vector2.Zero;

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
