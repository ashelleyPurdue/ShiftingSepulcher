using Godot;

namespace RandomDungeons
{
    [CustomNode]
    public class Area2DComponent : Area2D, IComponent<Node2D>
    {
        public Node2D Entity => GetParent<Node2D>();
        Node IComponent.Entity => GetParent();

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
