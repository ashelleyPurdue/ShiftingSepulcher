using System.Collections.Generic;
using Godot;

namespace ShiftingSepulcher
{
    [Tool]
    [CustomNode(parent: "Node2D", icon:"EditBezier")]
    public class BezierCurve2D : Node2D
    {
        [Export] public Vector2 StartPoint = Vector2.Zero;
        [Export] public Vector2 ControlPoint = Vector2.Up * 32;
        [Export] public Vector2 EndPoint = Vector2.Right * 32;
        [Export] public int NumVertices = 10;

        [Export] public float Width = 1;
        [Export] public Color Color = Colors.White;
        [Export] public bool Antialiased = false;

        // This is a field instead of a local variable so we can avoid allocating
        // a new array every frame.  Garbage collection is very expensive when
        // running in the browser, so we need to avoid creating unneccessary
        // GC pressure.
        private Vector2[] _points = new Vector2[] {};

        private const float HandleRadius = 16;
        private Position2D _startPointHandle;
        private Position2D _controlPointHandle;
        private Position2D _endPointHandle;
        private bool _lmbPressed = false;
        private bool _wasLmbPressed = false;

        public override void _Draw()
        {
            UpdatePointsArray();
            DrawPolyline(_points, Color, Width, Antialiased);
        }

        public override void _Process(float delta)
        {
            UpdateEditorHandles();
            Update();
        }

        private void UpdatePointsArray()
        {
            if (NumVertices != _points.Length)
            {
                _points = new Vector2[NumVertices];
            }

            for (int i = 0; i < NumVertices; i++)
            {
                float t = ((float)i) / (NumVertices - 1);
                _points[i] = SampleBezier(StartPoint, ControlPoint, EndPoint, t);
            }
        }

        private void UpdateEditorHandles()
        {
            if (!Engine.EditorHint)
                return;

            _wasLmbPressed = _lmbPressed;
            _lmbPressed = Input.IsMouseButtonPressed((int)ButtonList.Left);

            UpdateEditorHandle(ref _startPointHandle, ref StartPoint);
            UpdateEditorHandle(ref _controlPointHandle, ref ControlPoint);
            UpdateEditorHandle(ref _endPointHandle, ref EndPoint);
        }

        private void UpdateEditorHandle(ref Position2D handle, ref Vector2 point)
        {
            if (!Engine.EditorHint)
                return;

            // Ensure the handle exists
            if (!IsInstanceValid(handle))
            {
                handle = new Position2D();
                handle.Position = point;
                AddChild(handle);
            }

            // Select this handle when it's clicked on
            bool lmbJustPressed = _lmbPressed && !_wasLmbPressed;
            if (lmbJustPressed && DistanceToMouse(handle) < HandleRadius)
                SelectNodeInEditor(handle);

            // Transfer the handle's position to the point being edited
            point = handle.Position;
        }

        private float DistanceToMouse(Position2D handle)
        {
            return handle.GlobalPosition.DistanceTo(GetGlobalMousePosition());
        }

        private void SelectNodeInEditor(Node node)
        {
            if (!Engine.EditorHint)
                return;

            var editorInterface = GetTree().Root.FirstDescendantOfType<EditorInterface>();
            editorInterface.EditNode(node);
        }

        private static Vector2 SampleBezier(
            Vector2 startPoint,
            Vector2 controlPoint,
            Vector2 endPoint,
            float t
        )
        {
            var toControl = startPoint.LinearInterpolate(controlPoint, t);
            var toEnd = startPoint.LinearInterpolate(endPoint, t);
            return toControl.LinearInterpolate(toEnd, t);
        }
    }
}
