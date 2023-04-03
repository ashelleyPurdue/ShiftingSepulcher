using Godot;

namespace ShiftingSepulcher
{
    public class MinimapTest : Node2D
    {
        private Minimap _minimap => GetNode<Minimap>("%Minimap");
        private SeedInput _seedInput => GetNode<SeedInput>("%SeedInput");

        public void Regenerate()
        {
            var tree = DungeonTreeGenerator.GenerateUsingRuns(
                _seedInput.ParseSeedTextbox()
            );
            var layout = DungeonLayoutBuilder.LayoutFromTree(tree);

            _minimap.SetLayout(layout);
        }
    }
}
