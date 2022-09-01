namespace RandomDungeons.Graphs
{
    public class DungeonGraphDoor
    {
        public DungeonGraphRoom Destination;
        public bool IsLocked => LockId > 0;

        public int LockId = 0;
    }
}
