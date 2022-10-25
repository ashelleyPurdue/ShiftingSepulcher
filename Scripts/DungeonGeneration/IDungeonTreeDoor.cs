namespace RandomDungeons
{
    public interface IDungeonTreeDoor
    {
        DungeonTreeRoom Destination {get; set;}
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
}
