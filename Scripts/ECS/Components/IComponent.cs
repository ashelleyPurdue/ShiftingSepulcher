using Godot;

namespace RandomDungeons
{
    public interface IComponent
    {
        Node Entity {get;}

        /// <summary>
        /// Called when this component's entity has received the <see cref="Node._Ready"/>
        /// notification, indicating that all of its child nodes have been
        /// created, added as children, and are in the scene tree.
        ///
        /// Implement this when you need to do initialization that depends on
        /// other components in this entity.  This way, you can be sure that
        /// they exist yet.
        /// </summary>
        void _EntityReady();
    }

    public interface IComponent<TEntityNode> : IComponent
        where TEntityNode : Node
    {
        new TEntityNode Entity {get;}
    }
}
