using Godot;

namespace RandomDungeons
{
    [CustomNode]
    public class HealthPointsComponent : BaseComponent<Node>
    {
        [Signal] public delegate void TookDamage(int damage);
        [Signal] public delegate void TookDamageFromHitBox(HitBox hitBox);
        [Signal] public delegate void TookDamageNoParams();

        [Export] public int MaxHealth = 1;
        [Export] public float InvulnerabilityTime = 0;
        public int Health;

        public bool IsInvulnerable => _cooldownTimer > 0;
        private float _cooldownTimer = 0;

        public override void _Ready()
        {
            Health = MaxHealth;
        }

        public override void _PhysicsProcess(float delta)
        {
            if (IsInvulnerable)
                _cooldownTimer -= delta;
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
            _cooldownTimer = InvulnerabilityTime;

            EmitSignal(nameof(TookDamage), damage);
            EmitSignal(nameof(TookDamageNoParams));
        }

        public void OnTookDamageFromHitBox(HitBox hitBox)
        {
            TakeDamage(hitBox.Damage);
            EmitSignal(nameof(TookDamageFromHitBox), hitBox);
        }
    }
}
