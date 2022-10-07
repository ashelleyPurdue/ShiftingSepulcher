using Godot;
using RandomDungeons.Graphs;
using RandomDungeons.Utils;
namespace RandomDungeons.Nodes.UI.Widgets.Minimap
{
    public class RoomDisplay : Node2D
    {
        public void SetGraphRoom(DungeonGraphRoom graphRoom)
        {
            foreach (var dir in CardinalDirectionUtils.All())
            {
                var graphDoor = graphRoom.GetDoor(dir);
                var display = GetDoorDisplay(dir);

                display.SetGraphDoor(graphDoor);
            }
        }

        private DoorDisplay GetDoorDisplay(CardinalDirection dir)
        {
            return GetNode<DoorDisplay>($"%Doors/{dir}");
        }
    }
}
