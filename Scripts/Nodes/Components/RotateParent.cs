using Godot;

namespace RandomDungeons.Nodes.Components
{
    public class RotateParent : Node
    {
        [Export] public float RotSpeedDegrees = 360;
        [Export] public bool UsePhysicsProcess = false;

        private float _rotSpeed => Mathf.Deg2Rad(RotSpeedDegrees);
        private Node2D _parent => GetParent<Node2D>();

        public override void _Process(float delta)
        {
            if (!UsePhysicsProcess)
                _parent.GlobalRotation += _rotSpeed * delta;
        }

        public override void _PhysicsProcess(float delta)
        {
            if (UsePhysicsProcess)
                _parent.GlobalRotation += _rotSpeed * delta;
        }
    }
}
