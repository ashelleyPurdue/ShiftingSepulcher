using Godot;

namespace ShiftingSepulcher
{
    [Tool]
    public class ChompweedHeadModel : Node2D
    {
        private AnimationPlayer _mouthSlider => GetNode<AnimationPlayer>("%MouthOpenSlider");
        private Node2D _head => GetNode<Node2D>("%Head");

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
    }
}
