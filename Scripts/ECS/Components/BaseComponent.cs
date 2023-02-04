using Godot;

namespace RandomDungeons
{
    public abstract class BaseComponent<TEntityNode> : Node, IComponent<TEntityNode>
        where TEntityNode : Node
    {
        public TEntityNode Entity => GetParent<TEntityNode>();
        Node IComponent.Entity => GetParent();

        public virtual void _EntityReady() {}
    }
}
