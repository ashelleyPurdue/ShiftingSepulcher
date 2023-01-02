using Godot;

namespace RandomDungeons
{
    [Tool]
    public class DoorDisplay : Node2D
    {
        public void SetDoor(IDungeonTreeDoor door)
        {
            bool isWall = (door?.Destination == null);
            Visible = !isWall;

            GetNode<Node2D>("%OneWayIcon").Visible = door is IncomingShortcutDoor;
            GetNode<Node2D>("%Lock").Visible = door is LockedDoor;

            if (door is LockedDoor k)
            {
                GetNode<Node2D>("%Lock").Modulate = KeyColors.ForId(k.KeyId);
            }
        }
    }
}
