namespace ShiftingSepulcher
{
    public interface IDungeonTreeDoor
    {
        DungeonTreeRoom Destination {get; set;}
    }

    public class PlainDoor : IDungeonTreeDoor
    {
        public DungeonTreeRoom Destination {get; set;}
    }

    public class ChallengeDoor : IDungeonTreeDoor
    {
        public DungeonTreeRoom Destination {get; set;}
    }

    public class LockedDoor : IDungeonTreeDoor
    {
        public DungeonTreeRoom Destination {get; set;}
        public int KeyId;
    }

    public class IncomingShortcutDoor : IDungeonTreeDoor
    {
        public DungeonTreeRoom Destination {get; set;}
        public OutgoingShortcutDoor OtherSide {get; set;}
    }

    public class OutgoingShortcutDoor : IDungeonTreeDoor
    {
        public DungeonTreeRoom Destination {get; set;}
    }
}
