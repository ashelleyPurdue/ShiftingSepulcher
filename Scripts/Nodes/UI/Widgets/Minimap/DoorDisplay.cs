using Godot;
using RandomDungeons.Graphs;

namespace RandomDungeons.Nodes.UI.Widgets.Minimap
{
    public class DoorDisplay : Node2D
    {
        public void SetGraphDoor(IDungeonGraphDoor graphDoor)
        {
            bool isWall = (graphDoor.Destination == null);
            Visible = !isWall;
        }
    }
}
