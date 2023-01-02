namespace RandomDungeons
{
    public interface IDungeonGraphDoor
    {
        DungeonGraphRoom Destination {get; set;}
    }

    public class ChallengeDungeonGraphDoor : IDungeonGraphDoor
    {
        public DungeonGraphRoom Destination {get; set;}
        public bool IsOpened;
    }
}
