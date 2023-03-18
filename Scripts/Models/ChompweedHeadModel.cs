using Godot;

namespace ShiftingSepulcher
{
    [Tool]
    public class ChompweedHeadModel : Node2D
    {
        [Export] public float MouthOpen
        {
            get => _mouthOpen;
            set
            {
                _mouthOpen = value;

                var slider = GetNode<AnimationPlayer>("%MouthOpenSlider");
                slider.Stop(true);
                slider.Play("MouthOpen");
                slider.Advance(_mouthOpen);
                slider.Stop(false); // All the animation to be played with in
                                    // the editor
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

        private Node2D _head => GetNode<Node2D>("%Head");
    }
}
