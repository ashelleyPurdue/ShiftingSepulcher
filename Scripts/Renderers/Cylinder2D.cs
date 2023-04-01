using System;
using Godot;

namespace ShiftingSepulcher
{
    [Tool]
    [CustomNode("Node2D", "CylinderShape")]
    public class Cylinder2D : Node2D
    {
        [Export] public Color TopColor
        {
            get => _topColor;
            set => SetProperty(ref _topColor, value);
        }
        private Color _topColor = Colors.White;

        [Export] public Color BottomColor
        {
            get => _bottomColor;
            set => SetProperty(ref _bottomColor, value);
        }
        private Color _bottomColor = new Color("707070");

        [Export] public float TopRadius
        {
            get => _topRadius;
            set => SetProperty(ref _topRadius, value);
        }
        private float _topRadius = 16;

        [Export] public float Height
        {
            get => _height;
            set => SetProperty(ref _height, value);
        }
        private float _height = 32;

        [Export] public float BottomRadius
        {
            get => _bottomRadius;
            set => SetProperty(ref _bottomRadius, value);
        }
        private float _bottomRadius = 16;

        private Vector2[] _rectPoints = new Vector2[4];

        public override void _Draw()
        {
            DrawCircle(Vector2.Zero, BottomRadius, BottomColor);

            _rectPoints[0] = new Vector2(-TopRadius, -Height);
            _rectPoints[1] = new Vector2(TopRadius, -Height);
            _rectPoints[2] = new Vector2(BottomRadius, 0);
            _rectPoints[3] = new Vector2(-BottomRadius, 0);
            DrawColoredPolygon(_rectPoints, BottomColor);

            DrawCircle(Vector2.Up * Height, TopRadius, TopColor);
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
