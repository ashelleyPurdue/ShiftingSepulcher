using Godot;

namespace RandomDungeons
{
    public class RoomTemplateTester : Node2D
    {
        [Export] public PackedScene RoomTemplate;

        private SeedInput _seedInput => GetNode<SeedInput>("%SeedInput");
        private Node2D _roomLocation => GetNode<Node2D>("%RoomLocation");
        private IDungeonRoom _roomInstance;

        public override void _Ready()
        {
            Regenerate();
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
            graphRoom.RoomSeed = _seedInput.ParseSeedTextbox();

            _roomInstance.Populate(graphRoom);
        }
    }
}
