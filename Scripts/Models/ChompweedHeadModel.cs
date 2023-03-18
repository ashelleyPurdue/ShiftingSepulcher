using Godot;

namespace ShiftingSepulcher
{
    [Tool]
    public class ChompweedHeadModel : Node2D
    {
        private AnimationPlayer _mouthSlider => GetNode<AnimationPlayer>("%MouthOpenSlider");
        private Node2D _head => GetNode<Node2D>("%Head");
        private BezierCurve2D _stem => GetNode<BezierCurve2D>("%Stem");
        private Node2D _stemAttachPoint => GetNode<Node2D>(StemAttachPoint);

        [Export] public float MouthOpen
        {
            get => _mouthOpen;
            set
            {
                _mouthOpen = value;

                _mouthSlider.Stop(true);
                _mouthSlider.Play("MouthOpen");
                _mouthSlider.Advance(_mouthOpen);
                _mouthSlider.Stop(false); // Allow the animation to be played
                                          // with in the editor
            }
        }
        private float _mouthOpen = 0;

        [Export] public float HeightAboveGround
        {
            get => _heightAboveGround;
            set
            {
                _heightAboveGround = value;
                _head.Position = Vector2.Up * _heightAboveGround;
            }
        }
        private float _heightAboveGround;

        [Export] public NodePath StemAttachPoint = "";
        [Export] public float StemBend = -32;
        [Export] public bool StemVisible {get; set;} = true;

        public override void _Process(float delta)
        {
            _stem.Visible = StemVisible;

            if (!string.IsNullOrEmpty(StemAttachPoint))
            {
                _stem.StartPoint = ToLocal(_stemAttachPoint.GlobalPosition);
                _stem.ControlPoint = Vector2.Right * StemBend;
                _stem.EndPoint = _head.Position;
            }
        }
    }
}
