namespace RandomDungeons.DungeonTrees
{
    public interface IDungeonTreeDoor
    {
        DungeonTreeRoom Destination {get; set;}
    }

    public class ChallengeDoor : IDungeonTreeDoor
    {
        public DungeonTreeRoom Destination {get; set;}
    }
}