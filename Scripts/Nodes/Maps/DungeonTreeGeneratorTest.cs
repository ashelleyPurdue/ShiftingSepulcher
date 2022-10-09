using Godot;
using RandomDungeons.DungeonTrees;
using RandomDungeons.Nodes.UI.Widgets;

namespace RandomDungeons.Nodes.Maps
{
    public class DungeonTreeGeneratorTest : Control
    {
        private SeedInput _seedInput => GetNode<SeedInput>("%SeedInput");
        private Tree _treeDisplay => GetNode<Tree>("%TreeDisplay");

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
    }
}