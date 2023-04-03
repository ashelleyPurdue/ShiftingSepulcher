using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace ShiftingSepulcher
{
    public class DungeonRoomFactory : Node
    {
        [Export] public PackedScene EmptyRoom;

        public IDungeonRoom BuildRoom(DungeonLayoutRoom layoutRoom)
        {
            var spawnTable = GetChallengeSpawnTable(layoutRoom.TreeRoom.ChallengeType);

            if (spawnTable == null)
                return UseTemplate(EmptyRoom, layoutRoom);

            return SpawnFromTable(spawnTable, layoutRoom);
        }

        private IDungeonRoom SpawnFromTable(SpawnTable table, DungeonLayoutRoom layoutRoom)
        {
            var rng = new Random(layoutRoom.TreeRoom.RoomSeed);
            var realRoom = table.Spawn<IDungeonRoom>(rng);
            realRoom.Populate(layoutRoom);

            return realRoom;
        }

        private IDungeonRoom UseTemplate(PackedScene template, DungeonLayoutRoom layoutRoom)
        {
            var realRoom = template.Instance<IDungeonRoom>();
            realRoom.Populate(layoutRoom);
            return realRoom;
        }

        private SpawnTable GetChallengeSpawnTable(ChallengeType challengeType)
        {
            return GetNode<SpawnTable>($"%ChallengeTypeSpawnTables/{challengeType}");
        }
    }
}
