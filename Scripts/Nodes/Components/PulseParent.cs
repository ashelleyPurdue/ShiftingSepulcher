using Godot;

namespace RandomDungeons.Nodes.Components
{
    public class PulseParent : Node
    {
        [Export] public float MinScale = 0;
        [Export] public float MaxScale = 1;
        [Export] public float PeriodSeconds = 1;
        [Export] public float MinPeriodOffset = 0;
        [Export] public float MaxPeriodOffset = 0;

        [Export] public bool UsePhysicsProcess = false;

        private float _theta = 0;
        private Node2D _parent => GetParent<Node2D>();

        public override void _Ready()
        {
            _theta = Mathf.Lerp(MinPeriodOffset, MaxPeriodOffset, GD.Randf());
        }

        public override void _Process(float delta)
        {
            if (!UsePhysicsProcess)
                Pulse(delta);
        }

        public override void _PhysicsProcess(float delta)
        {
            if (UsePhysicsProcess)
                Pulse(delta);
        }

        private void Pulse(float delta)
        {
            float speed = 2 * Mathf.Pi / PeriodSeconds;
            _theta += delta * speed;

            float t = (Mathf.Cos(_theta) + 1) / 2;
            float scale = Mathf.Lerp(MinScale, MaxScale, t);
            _parent.Scale = Vector2.One * scale;
        }
    }
}
