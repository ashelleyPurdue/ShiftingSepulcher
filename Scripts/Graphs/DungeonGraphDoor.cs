namespace RandomDungeons.Graphs
{
    public class DungeonGraphDoor : IDungeonGraphDoor
    {
        public DungeonGraphRoom Destination {get; set;}
    }

    public class KeyDungeonGraphDoor : IDungeonGraphDoor
    {
        public DungeonGraphRoom Destination {get; set;}
        public int KeyId;

        public KeyDungeonGraphDoor(int keyId)
        {
            KeyId = keyId;
        }
    }
}
