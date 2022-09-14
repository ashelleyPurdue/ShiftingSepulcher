using Godot;

using RandomDungeons.Graphs;

namespace RandomDungeons.Nodes.DungeonRooms
{
    public class DungeonRoomFactory : Node
    {
        [Export] public PackedScene LegacyRoom;
        [Export] public PackedScene EmptyRoom;
        [Export] public PackedScene SingleZombieRoom;

        public IDungeonRoom BuildRoom(DungeonGraphRoom graphRoom)
        {
            switch (graphRoom.ChallengeType)
            {
                case ChallengeType.None: return BuildEmptyRoom(graphRoom);
                case ChallengeType.Combat: return BuildCombatRoom(graphRoom);
                default: return BuildLegacyRoom(graphRoom);
            }
        }

        private IDungeonRoom BuildEmptyRoom(DungeonGraphRoom graphRoom)
        {
            var realRoom = EmptyRoom.Instance<IDungeonRoom>();
            realRoom.Populate(graphRoom);
            return realRoom;
        }

        private IDungeonRoom BuildCombatRoom(DungeonGraphRoom graphRoom)
        {
            var realRoom = SingleZombieRoom.Instance<IDungeonRoom>();
            realRoom.Populate(graphRoom);
            return realRoom;
        }

        private IDungeonRoom BuildLegacyRoom(DungeonGraphRoom graphRoom)
        {
            var realRoom = LegacyRoom.Instance<IDungeonRoom>();
            realRoom.Populate(graphRoom);
            return realRoom;
        }
    }
}
