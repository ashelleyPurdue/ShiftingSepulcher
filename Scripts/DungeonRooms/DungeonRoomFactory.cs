using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace ShiftingSepulcher
{
    public class DungeonRoomFactory : Node
    {
        [Export] public PackedScene EmptyRoom;
        [Export] public PackedScene KeyRoom;

        [Export] public SceneSpawnTable FillerRoomTable;
        [Export] public SceneSpawnTable CombatRoomTable;
        [Export] public SceneSpawnTable PuzzleRoomTable;
        [Export] public SceneSpawnTable BossRoomTable;

        public IDungeonRoom BuildRoom(DungeonLayoutRoom layoutRoom)
        {
            switch (layoutRoom.TreeRoom.ChallengeType)
            {
                case ChallengeType.None: return SpawnFromTable(FillerRoomTable, layoutRoom);
                case ChallengeType.Loot: return UseTemplate(KeyRoom, layoutRoom);
                case ChallengeType.Combat: return SpawnFromTable(CombatRoomTable, layoutRoom);
                case ChallengeType.Puzzle: return SpawnFromTable(PuzzleRoomTable, layoutRoom);
                case ChallengeType.Boss: return SpawnFromTable(BossRoomTable, layoutRoom);

                default: return UseTemplate(EmptyRoom, layoutRoom);
            }
        }

        private IDungeonRoom SpawnFromTable(SceneSpawnTable table, DungeonLayoutRoom layoutRoom)
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
    }
}
