using Godot;

namespace RandomDungeons
{
    public class RotateParentTowardPoint : Node
    {
        [Export] public NodePath TargetPoint;
        [Export] public float OffsetDegrees;

        public override void _Process(float delta)
        {
            var targetNode = GetNode<Node2D>(TargetPoint);
            var parent = GetParent<Node2D>();

            var angle = parent.GlobalPosition.AngleToPoint(targetNode.GlobalPosition);
            angle += Mathf.Deg2Rad(OffsetDegrees);

            parent.GlobalRotation = angle;
        }
    }
}
