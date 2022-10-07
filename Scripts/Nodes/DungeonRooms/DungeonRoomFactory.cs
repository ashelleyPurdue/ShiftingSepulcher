using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

using RandomDungeons.Graphs;
using RandomDungeons.Utils;

namespace RandomDungeons.Nodes.DungeonRooms
{
    public class DungeonRoomFactory : Node
    {
        private const string RoomTemplatesFolder = "res://Scenes/Prefabs/DungeonRoomTemplates";
        private const string DictHintString = "The key is the name of a .tscn file, minus the file extension, relative to " + RoomTemplatesFolder;

        [Export] public PackedScene EmptyRoom;
        [Export] public PackedScene KeyRoom;

        [Export(hintString: DictHintString)]
        public Dictionary<string, int> FillerRoomWeights;

        [Export(hintString: DictHintString)]
        public Dictionary<string, int> CombatRoomWeights;

        [Export(hintString: DictHintString)]
        public Dictionary<string, int> PuzzleRoomWeights;

        [Export(hintString: DictHintString)]
        public Dictionary<string, int> BossRoomWeights;

        public IDungeonRoom BuildRoom(DungeonGraphRoom graphRoom)
        {
            switch (graphRoom.ChallengeType)
            {
                case ChallengeType.None: return PickTemplateFrom(FillerRoomWeights, graphRoom);
                case ChallengeType.Loot: return UseTemplate(KeyRoom, graphRoom);
                case ChallengeType.Combat: return PickTemplateFrom(CombatRoomWeights, graphRoom);
                case ChallengeType.Puzzle: return PickTemplateFrom(PuzzleRoomWeights, graphRoom);
                case ChallengeType.Boss: return PickTemplateFrom(BossRoomWeights, graphRoom);

                default: return UseTemplate(EmptyRoom, graphRoom);
            }
        }

        private IDungeonRoom BuildPuzzleRoom(DungeonGraphRoom graphRoom)
        {
            return PickTemplateFrom(PuzzleRoomWeights, graphRoom);
        }

        private IDungeonRoom PickTemplateFrom(
            Dictionary<string, int> templateWeights,
            DungeonGraphRoom graphRoom
        )
        {
            var weights = templateWeights
                .Select(kvp => (value: kvp.Key, weight: kvp.Value))
                .ToArray();

            var rng = new Random(graphRoom.RoomSeed);
            string templatePath = $"{RoomTemplatesFolder}/{rng.PickFromWeighted(weights)}.tscn";
            var template = GD.Load<PackedScene>(templatePath);

            return UseTemplate(template, graphRoom);
        }

        private IDungeonRoom UseTemplate(PackedScene template, DungeonGraphRoom graphRoom)
        {
            var realRoom = template.Instance<IDungeonRoom>();
            realRoom.Populate(graphRoom);
            return realRoom;
        }
    }
}
