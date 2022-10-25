using System;
using Godot;

namespace RandomDungeons
{
    public class DungeonTreeGeneratorTest : Node
    {
        private SeedInput _seedInput => GetNode<SeedInput>("%SeedInput");
        private Node _templates => GetNode<Node>("%TreeTemplates");
        private ItemList _templateSelector => GetNode<ItemList>("%TemplateSelector");
        private Tree _treeDisplay => GetNode<Tree>("%TreeDisplay");
        private Minimap _minimap => GetNode<Minimap>("%Minimap");

        public override void _Ready()
        {
            UpdateTemplateList();
            Regenerate();
        }

        public void Regenerate()
        {
            int seed = _seedInput.ParseSeedTextbox();
            var template = GetSelectedTemplate();

            DungeonTreeRoom tree;

            if (template == null)
            {
                tree = DungeonTreeGenerator.GenerateUsingRuns(
                    seed,
                    numRuns: 5,
                    minRunLength: 5,
                    maxRunLength: 5
                );
            }
            else
            {
                tree = template.ToDungeonTree(new Random(seed));
            }

            UpdateTreeDisplay(tree);
            UpdateMinimap(tree);
        }

        private DungeonTreeTemplate GetSelectedTemplate()
        {
            if (!_templateSelector.IsAnythingSelected())
                return null;

            int selectedIndex = _templateSelector.GetSelectedItems()[0];
            if (selectedIndex == 0)
                return null;

            // Subtracting 1 from the index to account for the "none" option
            return _templates.GetChild<DungeonTreeTemplate>(selectedIndex - 1);
        }

        private void UpdateTemplateList()
        {
            _templateSelector.Items.Clear();
            _templateSelector.AddItem("None (build a tree randomly)");

            foreach (var templateNode in _templates.GetChildren())
            {
                string name = ((Node)templateNode).Name;
                _templateSelector.AddItem(name);
            }
        }

        private void UpdateTreeDisplay(DungeonTreeRoom tree)
        {
            _treeDisplay.Clear();
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
            var layout = DungeonLayoutBuilder.LayoutFromTree(tree);
            var graph = DungeonGraphBuilder.BuildFromLayout(layout);
            _minimap.SetGraph(graph);
        }
    }
}
