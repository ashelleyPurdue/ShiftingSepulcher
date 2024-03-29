using Godot;

namespace ShiftingSepulcher
{
    [Tool]
    [CustomNode(parent: "Node2D", icon: "CircleShape2D")]
    public class Circle2D : Node2D
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

        private float _radius;
        private Color _color;

        public Circle2D()
        {
            Radius = 32;
            Color = Colors.White;
        }

        public override void _Draw()
        {
            DrawCircle(Vector2.Zero, Radius, Color);
        }
    }
}
