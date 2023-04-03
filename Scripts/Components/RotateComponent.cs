using Godot;

namespace ShiftingSepulcher
{
    public class RotateComponent : BaseComponent<Node2D>
    {
        [Export] public float RotSpeedDegrees = 360;
        [Export] public bool UsePhysicsProcess = false;

        private float _rotSpeed => Mathf.Deg2Rad(RotSpeedDegrees);

        public override void _Process(float delta)
        {
            if (!UsePhysicsProcess)
                Entity.GlobalRotation += _rotSpeed * delta;
        }

        public override void _PhysicsProcess(float delta)
        {
            if (UsePhysicsProcess)
                Entity.GlobalRotation += _rotSpeed * delta;
        }
    }
}
