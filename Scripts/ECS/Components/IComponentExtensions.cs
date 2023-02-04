using Godot;

namespace RandomDungeons
{
    public static class IComponentExtensions
    {
        /// <summary>
        /// Returns the first sibling component of the given type, or null
        /// if no sibling of that type exists.
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        public static TComponent GetComponent<TComponent>(this IComponent me)
            where TComponent : class, IComponent
        {
            foreach (var child in me.Entity.EnumerateChildren())
            {
                if (child is TComponent c)
                    return c;
            }

            return null;
        }

        /// <summary>
        /// Returns true if a sibling component of the given type exists
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        public static bool HasComponent<TComponent>(this IComponent me)
            where TComponent : class, IComponent
        {
            return me.GetComponent<TComponent>() != null;
        }

        /// <summary>
        /// Returns true if a sibling component of the given type exists.
        /// If so, "c" will be set to that component.  Otherwise, "c" will be
        /// set to null.
        /// </summary>
        /// <param name="c"></param>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        public static bool HasComponent<TComponent>(this IComponent me, out TComponent c)
            where TComponent : class, IComponent
        {
            c = me.GetComponent<TComponent>();
            return c != null;
        }
    }
}
