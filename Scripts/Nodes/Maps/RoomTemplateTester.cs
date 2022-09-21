using Godot;
using RandomDungeons.Graphs;
using RandomDungeons.Nodes.DungeonRooms;
using RandomDungeons.Utils;

namespace RandomDungeons.Nodes.Maps
{
    public class RoomTemplateTester : Node2D
    {
        [Export] public PackedScene RoomTemplate;

        private LineEdit _seedTextBox => GetNode<LineEdit>("%SeedTextbox");
        private Node2D _roomLocation => GetNode<Node2D>("%RoomLocation");
        private IDungeonRoom _roomInstance;

        public override void _Ready()
        {
            RandomizeSeed();
            Regenerate();
        }

        public void RandomizeSeed()
        {
            GD.Randomize();
            _seedTextBox.Text = "" + GD.Randi();
        }

        public void Regenerate()
        {
            // Remove the old room and replace it with a new one
            if (_roomInstance != null)
            {
                _roomLocation.RemoveChild(_roomInstance.Node);
                _roomInstance.Node.QueueFree();
            }

            _roomInstance = RoomTemplate.Instance<IDungeonRoom>();
            _roomLocation.AddChild(_roomInstance.Node);

            // Populate it
            var graph = new DungeonGraph();
            var graphRoom = graph.CreateRoom(Vector2i.Zero, 0);
            graphRoom.RoomSeed = ParseSeedTextbox();

            _roomInstance.Populate(graphRoom);
        }

        private int ParseSeedTextbox()
        {
            bool isNumber = int.TryParse(_seedTextBox.Text, out int seed);
            if (!isNumber)
                seed = (int)_seedTextBox.Text.Hash();

            return seed;
        }
    }
}
