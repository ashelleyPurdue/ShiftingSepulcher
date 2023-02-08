using System.Collections.Generic;
using Godot;

namespace RandomDungeons
{
    public static class IComponentExtensions
    {
        /// <summary>
        /// Yields all components on the given entity.
        /// If called on an <see cref="IComponent"/> instead of an entity, it
        /// will yield all _sibling_ components, including itself.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static IEnumerable<IComponent> EnumerateComponents(this Node entity)
        {
            // If this node is a component, search for siblings
            if (entity is IComponent nonEntity)
                entity = nonEntity.Entity;

            foreach (var child in entity.EnumerateChildren())
            {
                if (child is IComponent c)
                    yield return c;
            }
        }

        /// <summary>
        /// Returns the first component of the given type, or null
        /// if none of that type exists.
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

            foreach (var child in entity.EnumerateComponents())
            {
                if (child is TComponent c)
                    return c;
            }

            return null;
        }

        /// <summary>
        /// Yields all components of the given type, or null
        /// if none of that type exists.
        ///
        /// If this node is an <see cref="IComponent"/>, it will instead search
        /// for _sibling_ components.
        ///
        /// Use this instead of LINQ's ".Where()" method to avoid allocations.
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        public static IEnumerable<TComponent> GetComponents<TComponent>(this Node entity)
            where TComponent : class, IComponent
        {
            if (entity is IComponent nonEntity)
                entity = nonEntity.Entity;

            foreach (var child in entity.EnumerateComponents())
            {
                if (child is TComponent c)
                    yield return c;
            }
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
