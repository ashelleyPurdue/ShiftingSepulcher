using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace RandomDungeons.Utils
{
    public static class NodeExtensions
    {
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
