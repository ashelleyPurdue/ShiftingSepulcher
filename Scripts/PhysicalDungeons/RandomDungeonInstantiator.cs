using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace RandomDungeons
{
    public class RandomDungeonInstantiator : Node
    {
        [Export] public bool AlwaysUseTreeTemplate = false;


        private DungeonRoomFactory _roomFactory => GetNode<DungeonRoomFactory>("%RoomFactory");
        private RoomTransitionManager _transitionManager => GetNode<RoomTransitionManager>("%RoomTransitionManager");
        private IEnumerable<DungeonTreeTemplate> _treeTemplates => GetNode("%TreeTemplates")
            .GetChildren()
            .Cast<DungeonTreeTemplate>();

        public void Generate()
        {
            // Generate a dungeon graph
            GD.Print(TitleScreen.ChosenSeed);

            var tree = GenerateTree(TitleScreen.ChosenSeed);
            var layout = DungeonLayoutBuilder.LayoutFromTree(tree);
            var graph = DungeonGraphBuilder.BuildFromLayout(layout);

            // Create a "real" version of each room, but don't add it to the
            // scene yet.  We'll add it to the scene later, when the player
            // actually _enters_ it.
            var treeRoomToRoom2D = new Dictionary<DungeonTreeRoom, Room2D>();
            foreach (var coordinates in graph.AllRoomCoordinates())
            {
                var layoutRoom = new DungeonLayoutRoom(layout, coordinates);
                var realRoom = _roomFactory.BuildRoom(layoutRoom);

                treeRoomToRoom2D[layoutRoom.TreeRoom] = (Room2D)realRoom;
            }

            // Connect all the doors
            var shortcutDoorMap = new ShortcutDoorMap();
            foreach (var realRoom in treeRoomToRoom2D.Values)
            {
                ((IDungeonRoom)realRoom).ConnectDoors(treeRoomToRoom2D, shortcutDoorMap);
            }

            // Connect all the one-way doors
            foreach (var kvp in shortcutDoorMap.IncomingFakeToReal)
            {
                var incomingFake = kvp.Key;
                var incomingReal = kvp.Value;
                var outgoingFake = shortcutDoorMap.IncomingFakeToOutgoingFake[incomingFake];
                var outgoingReal = shortcutDoorMap.OutgoingFakeToReal[outgoingFake];

                incomingReal.OpenSide = outgoingReal;
            }

            var startRoom = layout.RoomAt(Vector2i.Zero);
            _transitionManager.StartDungeon(
                startRoom: treeRoomToRoom2D[startRoom],
                roomsToRespawn: treeRoomToRoom2D.Values
            );
        }

        private DungeonTreeRoom GenerateTree(int seed)
        {
            var rng = new System.Random(seed);

            if (!_treeTemplates.Any())
                return GenerateTreeWithoutTemplate(seed);

            if (AlwaysUseTreeTemplate)
                return GenerateTreeUsingTemplate(rng);

            // Bias towards _not_ using a template.  Otherwise, templates will
            // appear way too commonly
            return rng.Next(0, 4) == 0
                ? GenerateTreeUsingTemplate(rng)
                : GenerateTreeWithoutTemplate(seed);
        }

        private DungeonTreeRoom GenerateTreeWithoutTemplate(int seed)
        {
            GD.Print("Generating a tree without using a template");
            return DungeonTreeGenerator.GenerateUsingRuns(
                seed: seed,
                minRunLength: 3,
                maxRunLength: 5,
                numRuns: 6
            );
        }

        private DungeonTreeRoom GenerateTreeUsingTemplate(Random rng)
        {
            var chosenTemplate = rng.PickFrom(_treeTemplates);

            GD.Print($"Generating a tree using the {chosenTemplate.Name} template");
            return chosenTemplate.ToDungeonTree(rng);
        }
    }
}
