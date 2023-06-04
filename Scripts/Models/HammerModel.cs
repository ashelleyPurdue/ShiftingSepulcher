using Godot;

namespace ShiftingSepulcher
{
    [Tool]
    public class HammerModel : Node2D
    {
        [Export] public float HeadRadius = 8;
        [Export] public float HandleLength = 32;

        [Export] public float HandleThickness = 1.1f;

        [Export] public float HeadRotationDeg = 0;
        [Export]
        public float HeadTwistAngleDeg
        {
            get => _headTwistAngleDeg;
            set
            {
                _headTwistAngleDeg = value;
                SetHeadAngle((_swingPercent * 180) + _headTwistAngleDeg);
            }
        }
        private float _headTwistAngleDeg = 0;

        [Export] public float SwingPercent
        {
            get => _swingPercent;
            set
            {
                _swingPercent = value;
                SetHeadAngle((_swingPercent * 180) + _headTwistAngleDeg);
            }
        }
        private float _swingPercent = 0;

        [Export] public Color HandColor = Colors.DarkGray;
        [Export] public float HandRadius = 3.5f;
        [Export] public float LeftHandHeightPercent = 0.5f;
        [Export] public float RightHandHeightPercent = 0;

        private Node2D _head => GetNode<Node2D>("%Head");
        private AnimationPlayer _headAngleSlider => GetNode<AnimationPlayer>("%HeadAngleSlider");

        public override void _Draw()
        {
            DrawLine(
                Vector2.Zero,
                _head.Position,
                Colors.White,
                HandleThickness
            );

            DrawCircle(
                HandHeightToPos(LeftHandHeightPercent),
                HandRadius,
                HandColor
            );

            DrawCircle(
                HandHeightToPos(RightHandHeightPercent),
                HandRadius,
                HandColor
            );
        }

        public override void _Process(float delta)
        {
            _head.Scale = Vector2.One * HeadRadius;

            var headStartPos = Vector2.Up * HandleLength;
            var headEndPos = Vector2.Down * HandleLength;
            _head.Position = headStartPos.LinearInterpolate(
                headEndPos,
                Mathf.Clamp(SwingPercent, 0, 1)
            );

            _head.RotationDegrees = HeadRotationDeg;

            Update();
        }

        private void SetHeadAngle(float headAngleDeg)
        {
            float percent = headAngleDeg / 180;
            percent = Mathf.PosMod(percent, 1);

            _headAngleSlider.Stop(true);
            _headAngleSlider.Play("Angle");
            _headAngleSlider.Advance(percent);
            _headAngleSlider.Stop(false); // Allow the animation to be played
                                          // with in the editor
        }

        private Vector2 HandHeightToPos(float handHeightPercent)
        {
            float handleLengthAtZeroSwing = HandleLength;
            float handleLengthAtFullSwing = -HandleLength;
            float handleLength = Mathf.Lerp(handleLengthAtZeroSwing, handleLengthAtFullSwing, SwingPercent);

            return Vector2.Up * handleLength * handHeightPercent;
        }
    }
}
