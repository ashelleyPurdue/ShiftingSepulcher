using Godot;
using RandomDungeons.Utils;

namespace RandomDungeons.Nodes.Elements
{
    public class PlayerSword : Node2D
    {
        public bool IsSwinging => _swingTimer > 0;

        private const float SwingDuration = 0.15f;
        private const float SwingAngleDeg = 90;
        private float SwingSpeed => SwingAngleDeg / SwingDuration;

        private float _swingTimer = 0;

        public override void _PhysicsProcess(float delta)
        {
            Visible = IsSwinging;
            GetNode<Area2D>("SwordArea").Monitoring  = IsSwinging;
            GetNode<Area2D>("SwordArea").Monitorable = IsSwinging;
            RotationDegrees += SwingSpeed * delta;
            _swingTimer -= delta;
        }

        public void StartSwinging(EightDirection dir)
        {
            _swingTimer = SwingDuration;
            RotationDegrees = dir.ToAngleDeg() - (SwingAngleDeg / 2);
        }
    }
}
