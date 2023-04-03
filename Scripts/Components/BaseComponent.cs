using Godot;

namespace ShiftingSepulcher
{
    public abstract class BaseComponent<TEntityNode> : Node, IComponent<TEntityNode>
        where TEntityNode : Node
    {
        public TEntityNode Entity => this.GetEntity();
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
