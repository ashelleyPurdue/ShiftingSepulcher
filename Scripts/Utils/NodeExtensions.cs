using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace RandomDungeons
{
    public static class NodeExtensions
    {
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
        public static T FindAncestor<T>(this Node node) where T : Node
        {
            var parent = node.GetParent();

            if (parent is T)
                return (T)parent;

            return parent?.FindAncestor<T>();
        }

        public static IEnumerable<Node> AllDescendants(this Node n)
        {
            foreach (var child in n.GetChildren())
            {
                var childNode = (Node)child;
                yield return childNode;

                foreach (var descendant in childNode.AllDescendants())
                    yield return descendant;
            }
        }

        /// <summary>
        /// Yields all descendants of this node that are of the specified type.
        /// </summary>
        /// <param name="n"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> AllDescendantsOfType<T>(this Node n)
        {
            return n.AllDescendants()
                .Where(d => d is T)
                .Cast<T>();
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
            return n.GetChildren()
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

            foreach (var child in node.GetChildren())
            {
                ((Node)child).SetPaused(paused);
            }
        }
    }
}
