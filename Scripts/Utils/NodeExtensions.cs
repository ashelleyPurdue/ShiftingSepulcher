using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace ShiftingSepulcher
{
    public static class NodeExtensions
    {
        /// <summary>
        /// A garbage-collector-friendly alternative to <see cref="Node.GetChildren"/>.
        ///
        /// If used directly in a foreach loop, no garbage will be created when
        /// this method is called.
        ///
        /// <see cref="Node.GetChildren"/> causes stuttering in the browser if
        /// you call it every frame, because it allocates the output array on
        /// the heap.  These arrays pile up until they force the garbage
        /// collector to run.
        ///
        /// <see cref="NodeExtensions.EnumerateChildren"/>, on the other hand,
        /// only allocates a tiny enumerator struct.  If that struct is used in
        /// a foreach loop(and not anywhere else), the compiler will put that
        /// struct on the stack instead of the heap, meaning no garbage is
        /// created.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static IEnumerable<Node> EnumerateChildren(this Node node)
        {
            int count = node.GetChildCount();
            for (int i = 0; i < count; i++)
            {
                yield return (Node)(node.GetChild(i));
            }
        }

        /// <summary>
        /// Returns the room that this node lives in
        /// </summary>
        /// <param name="node"></param>
        /// <typeparam name="Room2D"></typeparam>
        /// <returns></returns>
        public static Room2D GetRoom(this Node node)
        {
            return node.FindAncestor<Room2D>()
                ?? node.GetNode<Room2D>("/root/FallbackRoom2D");
            // Use the fallback room singleton if there is no Room2D ancestor.
            // This way, it'll still work when testing scenes in isolation.
        }

        /// <summary>
        /// Returns the closest ancestor node of the given type.
        /// </summary>
        /// <param name="node"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T FindAncestor<T>(this Node node) where T : class
        {
            var parent = node.GetParent();

            if (parent is T match)
                return match;

            return parent?.FindAncestor<T>();
        }

        public static IEnumerable<Node> AllDescendants(this Node node)
        {
            var list = new List<Node>();
            Recursive(node);
            return list;

            void Recursive(Node n)
            {
                foreach (var child in n.EnumerateChildren())
                {
                    var childNode = (Node)child;
                    list.Add(childNode);
                    Recursive(childNode);
                }
            }
        }

        /// <summary>
        /// Yields all descendants of this node that are of the specified type.
        /// </summary>
        /// <param name="n"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> AllDescendantsOfType<T>(this Node node)
        {
            var list = new List<T>();
            Recursive(node);
            return list;

            void Recursive(Node n)
            {
                foreach (var child in n.EnumerateChildren())
                {
                    if (child is T childAsT)
                        list.Add(childAsT);

                    Recursive((Node)child);
                }
            }
        }

        /// <summary>
        /// Returns the first descendant of the given type, using breadth-first
        /// search.  Returns null if not found.
        /// </summary>
        /// <param name="node"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T FirstDescendantOfType<T>(this Node node)
        {
            var visitQueue = new Queue<Node>();
            visitQueue.Enqueue(node);

            while (visitQueue.Count > 0)
            {
                Node n = visitQueue.Dequeue();

                if (n is T result)
                    return result;

                foreach (var child in n.EnumerateChildren())
                {
                    visitQueue.Enqueue(child);
                }
            }

            return default;
        }

        /// <summary>
        /// Returns the only immediate child of the given type.
        /// If there is no immediate child with that type, or if there is more
        /// than one, throws an error.
        /// </summary>
        /// <param name="n"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T SingleChildOfType<T>(this Node n)
        {
            return n.EnumerateChildren()
                .OfType<T>()
                .Single();
        }


        /// <summary>
        /// Returns true this node or one of its ancestors are in the given
        /// group
        /// </summary>
        public static bool IsAncestorInGroup(this Node n, string groupName)
        {
            if (n == null)
                return false;

            if (n.IsInGroup(groupName))
                return true;

            return n.GetParent().IsAncestorInGroup(groupName);
        }

        /// <summary>
        /// Enables (or disables) all processing for the given node and its
        /// descendants.  Use this when you want to pause specific nodes instead
        /// of pausing the entire scene tree.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="paused"></param>
        public static void SetPaused(this Node node, bool paused)
        {
            node.SetProcess(!paused);
            node.SetPhysicsProcess(!paused);
            node.SetProcessInput(!paused);
            node.SetProcessUnhandledInput(!paused);
            node.SetProcessUnhandledKeyInput(!paused);

            foreach (var child in node.EnumerateChildren())
            {
                ((Node)child).SetPaused(paused);
            }
        }

        /// <summary>
        /// Returns the position of the given descendant relative to this node,
        /// as if the descendant were a direct child.
        ///
        /// Throws an error if the given node is not a descendant of this node.
        /// </summary>
        /// <param name="descendant"></param>
        /// <returns></returns>
        public static Vector2 GetPosRelativeToAncestor(this Node2D descendant, Node2D ancestor)
        {
            var parent = descendant.GetParentOrNull<Node2D>();

            if (parent == null)
                throw new Exception($"Not a descendant of {ancestor}");

            // Base case: the descendant is already a direct child.
            if (ancestor == parent)
                return descendant.Position;

            // Recursive case: shift it by the parent's relative pos.
            // TODO: Also take rotation and scale into consideration
            return parent.GetPosRelativeToAncestor(ancestor) + descendant.Position;
        }
    }
}
