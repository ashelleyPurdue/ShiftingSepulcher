using Godot;
using RandomDungeons.Graphs;
using RandomDungeons.Utils;
namespace RandomDungeons.Nodes.UI.Widgets.Minimap
{
    [Tool]
    public class RoomDisplay : Node2D
    {
        private Node2D _bossIcon => GetNode<Node2D>("%BossIcon");
        private Node2D _keyIcon => GetNode<Node2D>("%KeyIcon");


        public void SetGraphRoom(DungeonGraphRoom graphRoom)
        {
            _bossIcon.Visible = graphRoom.ChallengeType == ChallengeType.Boss;

            _keyIcon.Visible = graphRoom.KeyId > 0;
            _keyIcon.Modulate = KeyColors.ForId(graphRoom.KeyId);

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
