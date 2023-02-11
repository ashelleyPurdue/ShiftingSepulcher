using Godot;

namespace ShiftingSepulcher
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
            var layout = new DungeonLayout().WithRoomAt(Vector2i.Zero, new DungeonTreeRoom());
            var layoutRoom = layout.RoomAt(Vector2i.Zero);
            _roomInstance.Populate(layoutRoom);
        }
    }
}
