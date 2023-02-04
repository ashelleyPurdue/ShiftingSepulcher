using Godot;

namespace RandomDungeons
{
    public interface IEntity<TNode> where TNode : Node
    {
        TNode Node {get;}
    }

    public static class IEntityExtensions
    {
        /// <summary>
        /// Sends the _EntityReady() message to all components.
        /// Only use this inside an IEntity's Ready() method.  If I could make
        /// this method private, I would.
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="TNode"></typeparam>
        public static void SendEntityReady<TNode>(this IEntity<TNode> entity)
            where TNode : Node
        {
            foreach (var child in entity.Node.EnumerateChildren())
            {
                if (child is IComponent component)
                    component._EntityReady();
            }
        }
    }
}
