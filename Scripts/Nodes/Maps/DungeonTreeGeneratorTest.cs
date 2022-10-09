using Godot;
using RandomDungeons.DungeonTrees;
using RandomDungeons.Nodes.UI.Widgets;
using RandomDungeons.Nodes.UI.Widgets.Minimap;

namespace RandomDungeons.Nodes.Maps
{
    public class DungeonTreeGeneratorTest : Node
    {
        private SeedInput _seedInput => GetNode<SeedInput>("%SeedInput");
        private Tree _treeDisplay => GetNode<Tree>("%TreeDisplay");
        private Minimap _minimap => GetNode<Minimap>("%Minimap");

        public override void _Ready()
        {
            Regenerate();
        }

        public void Regenerate()
        {
            _treeDisplay.Clear();

            var tree = DungeonTreeGenerator.GenerateUsingRuns(
                _seedInput.ParseSeedTextbox(),
                numRuns: 5,
                minRunLength: 5,
                maxRunLength: 5
            );

            UpdateTreeDisplay(tree);
            UpdateMinimap(tree);
        }

        private void UpdateTreeDisplay(DungeonTreeRoom tree)
        {
            Recursive(tree, _treeDisplay.GetRoot());

            void Recursive(DungeonTreeRoom room, TreeItem parent)
            {
                TreeItem treeItem = _treeDisplay.CreateItem(parent);
                treeItem.SetText(0, room.ChallengeType.ToString());

                foreach (var childDoor in room.ChildDoors)
                {
                    Recursive(childDoor.Destination, treeItem);
                }
            }
        }

        private void UpdateMinimap(DungeonTreeRoom tree)
        {
            var graph = Graphs.DungeonGraphBuilder.BuildFromTree(tree);
            _minimap.SetGraph(graph);
        }
    }
}