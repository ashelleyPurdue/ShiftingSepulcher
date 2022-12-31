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
            var graphRoomToDungeonRoom = new Dictionary<DungeonGraphRoom, IDungeonRoom>();
            var graphRoomToRoom2D = new Dictionary<DungeonGraphRoom, Room2D>();
            foreach (var coordinates in graph.AllRoomCoordinates())
            {
                var graphRoom = graph.GetRoom(coordinates);
                var realRoom = _roomFactory.BuildRoom(graphRoom);

                graphRoomToDungeonRoom[graphRoom] = realRoom;
                graphRoomToRoom2D[graphRoom] = (Room2D)realRoom;
            }

            // Connect all the doors
            foreach (var realRoom in graphRoomToDungeonRoom.Values)
            {
                realRoom.ConnectDoors(graphRoomToRoom2D);
            }

            _transitionManager.StartDungeon(
                startRoom: graphRoomToRoom2D[graph.StartRoom],
                roomsToRespawn: graphRoomToRoom2D.Values
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
