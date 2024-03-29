using Godot;

namespace ShiftingSepulcher
{
    [Tool]
    [CustomNode(parent: "Node2D", icon: "CircleShape2D")]
    public class Ring2D : Node2D
    {
        [Export] public float Radius
        {
            get => _radius;
            set
            {
                if (value == _radius)
                    return;

                _radius = value;
                Update();
            }
        }

        [Export] public float Thickness
        {
            get => _thickness;
            set
            {
                if (value == _thickness)
                    return;

                _thickness = value;
                Update();
            }
        }

        [Export] public Color Color
        {
            get => _color;
            set
            {
                if (value == _color)
                    return;

                _color = value;
                Update();
            }
        }

        [Export] public int PointCount
        {
            get => _pointCount;
            set
            {
                if (value == _pointCount)
                    return;

                _pointCount = value;
                Update();
            }
        }

        private float _radius = 16;
        private float _thickness = 1;
        private Color _color = Colors.White;
        private int _pointCount = 32;

        public override void _Draw()
        {
            DrawArc(
                Vector2.Zero,
                _radius,
                0,
                Mathf.Deg2Rad(360),
                _pointCount,
                _color,
                _thickness
            );
        }
    }
}
