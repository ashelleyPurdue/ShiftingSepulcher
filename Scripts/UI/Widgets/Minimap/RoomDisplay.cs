using Godot;
namespace RandomDungeons
{
    [Tool]
    public class RoomDisplay : Node2D
    {
        private Node2D _bossIcon => GetNode<Node2D>("%BossIcon");
        private Node2D _keyIcon => GetNode<Node2D>("%KeyIcon");

        public void SetRoom(DungeonLayoutRoom layoutRoom)
        {
            _bossIcon.Visible = layoutRoom.TreeRoom.ChallengeType == ChallengeType.Boss;

            _keyIcon.Visible = layoutRoom.TreeRoom.KeyId > 0;
            _keyIcon.Modulate = KeyColors.ForId(layoutRoom.TreeRoom.KeyId);

            foreach (var dir in CardinalDirectionUtils.All())
            {
                var graphDoor = layoutRoom.DoorAtDirection(dir);
                var display = GetDoorDisplay(dir);

                display.SetDoor(graphDoor);
            }
        }

        private DoorDisplay GetDoorDisplay(CardinalDirection dir)
        {
            return GetNode<DoorDisplay>($"%Doors/{dir}");
        }
    }
}
