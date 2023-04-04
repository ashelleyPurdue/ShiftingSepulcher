using Godot;

namespace ShiftingSepulcher
{
    [Tool]
    public class HammerModel : Node2D
    {
        [Export] public float HeadRadius = 8;
        [Export] public float HandleLength = 32;

        [Export] public float HandleThickness = 1.1f;


        [Export] public float SwingPercent
        {
            get => _swingPercent;
            set
            {
                _swingPercent = value;

                _headAngleSlider.Stop(true);
                _headAngleSlider.Play("Angle");
                _headAngleSlider.Advance(_swingPercent);
                _headAngleSlider.Stop(false); // Allow the animation to be played
                                              // with in the editor
            }
        }
        private float _swingPercent = 0;

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

            Update();
        }
    }
}
