using Godot;

namespace RandomDungeons
{
    public static class IComponentExtensions
    {
        /// <summary>
        /// Returns the first component of the given type, or null
        /// if no of that type exists.
        ///
        /// If this node is an <see cref="IComponent"/>, it will instead search
        /// for _sibling_ components.
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        public static TComponent GetComponent<TComponent>(this Node entity)
            where TComponent : class, IComponent
        {
            if (entity is IComponent component)
                return component.Entity.GetComponent<TComponent>();

            foreach (var child in entity.EnumerateChildren())
            {
                if (child is TComponent c)
                    return c;
            }

            return null;
        }

        /// <summary>
        /// Returns true if a sibling component of the given type exists
        ///
        /// If this node is an <see cref="IComponent"/>, it will instead search
        /// for _sibling_ components.
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        public static bool HasComponent<TComponent>(this Node entity)
            where TComponent : class, IComponent
        {
            if (entity is IComponent component)
                return component.Entity.HasComponent<TComponent>();

            return entity.GetComponent<TComponent>() != null;
        }

        /// <summary>
        /// Returns true if a sibling component of the given type exists.
        /// If so, "c" will be set to that component.  Otherwise, "c" will be
        /// set to null.
        ///
        /// If this node is an <see cref="IComponent"/>, it will instead search
        /// for _sibling_ components.
        /// </summary>
        /// <param name="c"></param>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        public static bool HasComponent<TComponent>(this Node entity, out TComponent c)
            where TComponent : class, IComponent
        {
            if (entity is IComponent component)
                return component.Entity.HasComponent<TComponent>(out c);

            c = entity.GetComponent<TComponent>();
            return c != null;
        }
    }
}
