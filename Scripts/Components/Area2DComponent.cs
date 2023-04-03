using Godot;

namespace ShiftingSepulcher
{
    [CustomNode(parent:"Area2D", icon: "Area2D")]
    public class Area2DComponent : Area2D, IComponent<Node2D>
    {
        public Node2D Entity => this.GetEntity();
        Node IComponent.Entity => this.GetEntity();

        Node IComponent.Node => this;

        public override sealed void _Ready()
        {
            Entity.Connect(
                signal: "ready",
                target: this,
                method: nameof(_EntityReady),
                flags: (uint)(ConnectFlags.Oneshot)
            );
        }

        public virtual void _EntityReady() {}
    }
}
