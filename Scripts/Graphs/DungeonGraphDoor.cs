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

    public class OneWayOpenSideGraphDoor : IDungeonGraphDoor
    {
        public DungeonGraphRoom Destination {get; set;}
        public OneWayClosedSideGraphDoor OtherSide {get; set;}
    }

    public class OneWayClosedSideGraphDoor : IDungeonGraphDoor
    {
        public DungeonGraphRoom Destination {get; set;}
        public OneWayOpenSideGraphDoor OtherSide {get; set;}
    }
}
