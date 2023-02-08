using Godot;

namespace RandomDungeons
{
    public abstract class BaseComponent<TEntityNode> : Node, IComponent<TEntityNode>
        where TEntityNode : Node
    {
        public TEntityNode Entity => GetParent<TEntityNode>();
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