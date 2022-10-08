using Godot;

using RandomDungeons.Graphs;
using RandomDungeons.Nodes.UI.Widgets;
using RandomDungeons.Nodes.UI.Widgets.Minimap;

namespace RandomDungeons.Nodes.Maps
{
    public class MinimapTest : Node2D
    {
        private Minimap _minimap => GetNode<Minimap>("%Minimap");
        private SeedInput _seedInput => GetNode<SeedInput>("%SeedInput");

        public void Regenerate()
        {
            var graph = DungeonGenerator.GenerateUsingRuns(
                seed: _seedInput.ParseSeedTextbox(),
                minRunSize: 3,
                maxRunSize: 5,
                numRooms: 25
            );
            _minimap.SetGraph(graph);
        }
    }
}
