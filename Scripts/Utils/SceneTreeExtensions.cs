using Godot;
using RandomDungeons.Nodes.Elements;

namespace RandomDungeons.Utils
{
    public static class SceneTreeExtensions
    {
        public static Player FindPlayer(this SceneTree tree)
        {
            return FindPlayerRecursive(tree.Root);

            Player FindPlayerRecursive(Node n)
            {
                foreach (var c in n.GetChildren())
                {
                    if (c is Player player)
                        return player;

                    var playerInChild = FindPlayerRecursive((Node)c);

                    if (playerInChild != null)
                        return playerInChild;
                }

                return null;
            }
        }
    }
}
