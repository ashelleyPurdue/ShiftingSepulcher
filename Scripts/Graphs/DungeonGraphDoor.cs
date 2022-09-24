namespace RandomDungeons.Graphs
{
    public interface IDungeonGraphDoor
    {
        DungeonGraphRoom Destination {get; set;}
    }

    public class DungeonGraphDoor : IDungeonGraphDoor
    {
        public DungeonGraphRoom Destination {get; set;}
    }

    public class ChallengeDungeonGraphDoor : IDungeonGraphDoor
    {
        public DungeonGraphRoom Destination {get; set;}
        public bool IsOpened;
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

        public bool IsOpened;
    }

    public class OneWayClosedSideGraphDoor : IDungeonGraphDoor
    {
        public DungeonGraphRoom Destination {get; set;}
        public OneWayOpenSideGraphDoor OtherSide {get; set;}

        public bool IsOpened => OtherSide.IsOpened;
    }
}
