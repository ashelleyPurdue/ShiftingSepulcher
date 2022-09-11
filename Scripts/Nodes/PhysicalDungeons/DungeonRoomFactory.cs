using Godot;
using RandomDungeons.Graphs;

namespace RandomDungeons.PhysicalDungeons
{
    public class DungeonRoomFactory : Node
    {
        [Export] public PackedScene LegacyRoom;

        public DungeonRoom BuildRoom(DungeonGraphRoom graphRoom)
        {
            // TODO: Delegate to different "sub" builders depending on the
            // the challenge type

            var realRoom = LegacyRoom.Instance<DungeonRoom>();
            realRoom.SetGraphRoom(graphRoom);
            return realRoom;
        }
    }
}
