namespace RandomDungeons.Graphs
{
    public class DungeonGraphDoor : IDungeonGraphDoor
    {
        public DungeonGraphRoom Destination {get; set;}
        public bool IsLocked => LockId > 0;

        public int LockId = 0;
    }
}
