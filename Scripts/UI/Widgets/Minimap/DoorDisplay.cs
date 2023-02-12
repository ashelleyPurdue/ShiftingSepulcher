using Godot;

namespace ShiftingSepulcher
{
    [Tool]
    public class DoorDisplay : Node2D
    {
        public void SetDoor(DungeonLayoutRoom room, IDungeonTreeDoor door)
        {
            if (door == null)
            {
                Visible = false;
                return;
            }
            Visible = true;

            GetNode<Node2D>("%OneWayIcon").Visible = door is IncomingShortcutDoor;
            GetNode<Node2D>("%Lock").Visible = door is LockedDoor;

            if (door is LockedDoor k)
            {
                GetNode<Node2D>("%Lock").Modulate = KeyColors.ForId(k.KeyId);
            }

            Vector3i destPos = room.Layout.CoordsOf(door.Destination);
            GetNode<Node2D>("%StairsUpIcon").Visible = destPos.z > room.Position.z;
            GetNode<Node2D>("%StairsDownIcon").Visible = destPos.z < room.Position.z;
        }
    }
}
