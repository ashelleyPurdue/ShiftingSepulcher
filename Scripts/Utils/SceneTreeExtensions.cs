using Godot;

namespace ShiftingSepulcher
{
    public static class SceneTreeExtensions
    {
        public static Player FindPlayer(this SceneTree tree)
        {
            return FindPlayerRecursive(tree.Root);

            Player FindPlayerRecursive(Node n)
            {
                foreach (var c in n.EnumerateChildren())
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

        public static PhysicsTimer CreatePhysicsTimer(
            this SceneTree tree,
            float duration
        )
        {
            var timer = new PhysicsTimer(duration);
            tree.Root.AddChild(timer);

            return timer;
        }
    }
}
