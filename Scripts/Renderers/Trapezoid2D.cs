using Godot;

namespace ShiftingSepulcher
{
    [Tool]
    [CustomNode("Node2D", "QuadShape")]
    public class Trapezoid2D : Node2D
    {
        [Export] public Color Color
        {
            get => _color;
            set => SetProperty(ref _color, value);
        }
        private Color _color = Colors.White;

        [Export] public float Height
        {
            get => _height;
            set => SetProperty(ref _height, value);
        }
        private float _height = 32;

        [Export] public float TopWidth
        {
            get => _topWidth;
            set => SetProperty(ref _topWidth, value);
        }
        private float _topWidth = 32;

        [Export] public float BottomWidth
        {
            get => _bottomWidth;
            set => SetProperty(ref _bottomWidth, value);
        }
        private float _bottomWidth = 32;

        private Vector2[] _rectPoints = new Vector2[4];

        public override void _Draw()
        {
            float h = Height / 2;
            float tw = TopWidth / 2;
            float bw = BottomWidth / 2;

            _rectPoints[0] = new Vector2(-tw, -h);
            _rectPoints[1] = new Vector2(tw, -h);
            _rectPoints[2] = new Vector2(bw, h);
            _rectPoints[3] = new Vector2(-bw, h);
            DrawColoredPolygon(_rectPoints, Color);
        }

        private void SetProperty<T>(ref T _storage, T value)
        {
            T prev = _storage;
            _storage = value;

            if (!prev.Equals(value))
                Update();
        }
    }
}
