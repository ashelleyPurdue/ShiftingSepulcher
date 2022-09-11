using Godot;

using RandomDungeons.Graphs;
using RandomDungeons.PhysicalDungeons;

namespace RandomDungeons.Nodes.DungeonRooms
{
    public class DungeonRoomFactory : Node
    {
        [Export] public PackedScene LegacyRoom;

        public IDungeonRoom BuildRoom(DungeonGraphRoom graphRoom)
        {
            // TODO: Delegate to different "sub" builders depending on the
            // the challenge type

            var realRoom = LegacyRoom.Instance<LegacyDungeonRoom>();
            realRoom.SetGraphRoom(graphRoom);
            return realRoom;
        }
    }
}
