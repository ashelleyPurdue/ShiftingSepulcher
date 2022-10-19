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
