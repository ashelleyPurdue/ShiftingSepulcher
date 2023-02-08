using Godot;

namespace RandomDungeons
{
    [CustomNode]
    public class RotateTowardPointComponent : BaseComponent<Node2D>
    {
        [Export] public NodePath TargetPoint;
        [Export] public float OffsetDegrees;

        public override void _Process(float delta)
        {
            var targetNode = GetNode<Node2D>(TargetPoint);

            var angle = Entity.GlobalPosition.AngleToPoint(targetNode.GlobalPosition);
            angle += Mathf.Deg2Rad(OffsetDegrees);

            Entity.GlobalRotation = angle;
        }
    }
}
