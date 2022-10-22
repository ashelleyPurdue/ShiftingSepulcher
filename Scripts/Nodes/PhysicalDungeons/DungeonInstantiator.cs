using System.Collections.Generic;
using System.Linq;
using Godot;

using RandomDungeons.DungeonTrees;
using RandomDungeons.DungeonLayouts;
using RandomDungeons.Graphs;
using RandomDungeons.Nodes.TreeTemplates;
using RandomDungeons.Nodes.DungeonRooms;
using RandomDungeons.Nodes.UI;
using RandomDungeons.Utils;

namespace RandomDungeons.PhysicalDungeons
{
    public class DungeonInstantiator : Node
    {
        [Export] public bool AlwaysUseTreeTemplate = false;

        private Node _treeTemplates => GetNode<Node>("%TreeTemplates");
        private DungeonRoomFactory _roomFactory => GetNode<DungeonRoomFactory>("%RoomFactory");
        private RoomTransitionManager _transitionManager => GetNode<RoomTransitionManager>("%RoomTransitionManager");

        public override void _Ready()
        {
            // Generate a dungeon graph
            GD.Print(TitleScreen.ChosenSeed);

            var tree = GenerateTree(TitleScreen.ChosenSeed);
            var layout = DungeonLayoutBuilder.LayoutFromTree(tree);
            var graph = DungeonGraphBuilder.BuildFromLayout(layout);

            // Create a "real" version of each room, but don't add it to the
            // scene yet.  We'll add it to the scene later, when the player
            // actually _enters_ it.
            var graphRoomToRealRoom = new Dictionary<DungeonGraphRoom, IDungeonRoom>();
            foreach (var coordinates in graph.AllRoomCoordinates())
            {
                var graphRoom = graph.GetRoom(coordinates);
                var realRoom = _roomFactory.BuildRoom(graphRoom);
                graphRoomToRealRoom[graphRoom] = realRoom;
            }

            _transitionManager.SetGraph(graph, graphRoomToRealRoom);
        }

        private DungeonTreeRoom GenerateTree(int seed)
        {
            var rng = new System.Random(seed);

            // Bias towards _not_ using a template.  Otherwise, templates will
            // appear way too commonly
            bool useTreeTemplate = AlwaysUseTreeTemplate || rng.Next(0, 4) == 0;
            if (!useTreeTemplate)
            {
                GD.Print("Generating a tree without using a template");
                return DungeonTreeGenerator.GenerateUsingRuns(
                    seed: seed,
                    minRunLength: 3,
                    maxRunLength: 5,
                    numRuns: 6
                );
            }

            var templateOptions = _treeTemplates
                .GetChildren()
                .Cast<DungeonTreeTemplate>();
            var chosenTemplate = rng.PickFrom(templateOptions);

            GD.Print($"Generating a tree using the {chosenTemplate.Name} template");
            return chosenTemplate.ToDungeonTree(rng);
        }
    }
}
