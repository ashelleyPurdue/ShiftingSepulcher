using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace ShiftingSepulcher
{
    [Tool]
    public class LineTrail2D : Node2D
    {
        [Export] public Color StartColor = Colors.Yellow;
        [Export] public Color EndColor = Colors.Yellow;
        [Export] public float StartWidth = 4;
        [Export] public float EndWidth = 0;
        [Export] public float PointDuration = 0.5f;
        [Export] public float PointsPerSecond = 60;
        [Export] public bool Active = true;

        private float _nextPointTimer;
        private List<Point> _globalPoints = new List<Point>();

        private struct Point
        {
            public Vector2 GlobalPos;
            public float Age;

            public Point(Vector2 pos)
            {
                GlobalPos = pos;
                Age = 0;
            }
        }

        public override void _Draw()
        {
            if (_globalPoints.Count < 2)
                return;

            var localPoints = _globalPoints
                .Select(p => ToLocal(p.GlobalPos))
                .ToArray();

            Vector2 lastPoint = Active
                ? Vector2.Zero
                : localPoints[0];

            for (int i = 0; i < localPoints.Length; i++)
            {
                Vector2 nextPoint = localPoints[i];

                float t = ((float)i) / localPoints.Length;

                float width = Mathf.Lerp(StartWidth, EndWidth, t);
                Color color = StartColor.LinearInterpolate(EndColor, t);

                DrawLine(lastPoint, nextPoint, color, width);
                lastPoint = nextPoint;
            }
        }

        public override void _Process(float delta)
        {
            Update();

            // Keep track of each point's age
            for (int i = 0; i < _globalPoints.Count; i++)
            {
                var point = _globalPoints[i];
                point.Age += delta;
                _globalPoints[i] = point;
            }

            // Spawn a new point every so often
            _nextPointTimer += delta;

            float pointInterval = 1f / PointsPerSecond;

            while (Active && _nextPointTimer >= pointInterval)
            {
                _globalPoints.Insert(0, new Point(GlobalPosition));
                _nextPointTimer -= pointInterval;
            }

            // Delete points that have outlived their usefulness
            while (_globalPoints.Any() && _globalPoints.Last().Age >= PointDuration)
                _globalPoints.RemoveAt(_globalPoints.Count - 1);
        }
    }
}
