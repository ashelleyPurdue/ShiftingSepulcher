namespace RandomDungeons.Graphs
{
    public class DungeonDoor
    {
        public DungeonRoom Destination;
        public bool IsLocked => LockId > 0;

        public int LockId = 0;
    }
}
