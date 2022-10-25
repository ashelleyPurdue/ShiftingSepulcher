using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace RandomDungeons
{
    public class DungeonRoomFactory : Node
    {
        [Export] public PackedScene EmptyRoom;
        [Export] public PackedScene KeyRoom;

        [Export] public SceneSpawnTable FillerRoomTable;
        [Export] public SceneSpawnTable CombatRoomTable;
        [Export] public SceneSpawnTable PuzzleRoomTable;
        [Export] public SceneSpawnTable BossRoomTable;

        public IDungeonRoom BuildRoom(DungeonGraphRoom graphRoom)
        {
            switch (graphRoom.ChallengeType)
            {
                case ChallengeType.None: return SpawnFromTable(FillerRoomTable, graphRoom);
                case ChallengeType.Loot: return UseTemplate(KeyRoom, graphRoom);
                case ChallengeType.Combat: return SpawnFromTable(CombatRoomTable, graphRoom);
                case ChallengeType.Puzzle: return SpawnFromTable(PuzzleRoomTable, graphRoom);
                case ChallengeType.Boss: return SpawnFromTable(BossRoomTable, graphRoom);

                default: return UseTemplate(EmptyRoom, graphRoom);
            }
        }

        private IDungeonRoom SpawnFromTable(SceneSpawnTable table, DungeonGraphRoom graphRoom)
        {
            var rng = new Random(graphRoom.RoomSeed);
            var realRoom = table.Spawn<IDungeonRoom>(rng);
            realRoom.Populate(graphRoom);

            return realRoom;
        }

        private IDungeonRoom UseTemplate(PackedScene template, DungeonGraphRoom graphRoom)
        {
            var realRoom = template.Instance<IDungeonRoom>();
            realRoom.Populate(graphRoom);
            return realRoom;
        }
    }
}
