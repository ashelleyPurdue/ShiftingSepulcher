using Godot;

namespace RandomDungeons.Nodes.Components
{
    public class HurtBox : Area2D
    {
        [Signal] public delegate void TookDamage(HitBox hitBox);

        public bool IsInvulnerable => _cooldownTimer > 0;
        private float _cooldownTimer = 0;

        public override void _PhysicsProcess(float delta)
        {
            if (IsInvulnerable)
                _cooldownTimer -= delta;
        }

        public void TakeDamage(HitBox hitBox)
        {
            _cooldownTimer = hitBox.InvlunerabilityTime;
            EmitSignal(nameof(TookDamage), hitBox);
        }
    }
}
