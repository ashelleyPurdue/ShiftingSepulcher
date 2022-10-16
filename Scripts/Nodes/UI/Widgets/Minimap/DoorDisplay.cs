using Godot;
using RandomDungeons.Graphs;
using RandomDungeons.Utils;

namespace RandomDungeons.Nodes.UI.Widgets.Minimap
{
    [Tool]
    public class DoorDisplay : Node2D
    {
        public void SetGraphDoor(IDungeonGraphDoor graphDoor)
        {
            bool isWall = (graphDoor.Destination == null);
            Visible = !isWall;

            GetNode<Node2D>("%OneWayIcon").Visible = graphDoor is OneWayClosedSideGraphDoor;
            GetNode<Node2D>("%Lock").Visible = graphDoor is KeyDungeonGraphDoor;

            if (graphDoor is KeyDungeonGraphDoor k)
            {
                GetNode<Node2D>("%Lock").Modulate = KeyColors.ForId(k.KeyId);
            }
        }
    }
}
