using Godot;

namespace RandomDungeons
{
    public class PipePuzzleCell : Node2D
    {
        [Export] public Color UnpoweredColor;
        [Export] public Color PoweredColor;

        public PipePuzzleGraph.Cell Cell;
        public bool IsPowered;

        private AnimationPlayer _animator => GetNode<AnimationPlayer>("%Animator");

        public override void _Process(float delta)
        {
            foreach (var dir in CardinalDirectionUtils.All())
            {
                var pipe = GetPipeDisplay(dir);
                pipe.Visible = Cell.IsDirectionOpen(dir);

                pipe.SelfModulate = IsPowered
                    ? PoweredColor
                    : UnpoweredColor;
            }
        }

        public void OnKnobHit(HitBox hitbox)
        {
            Cell.RotateClockwise();
            _animator.Play("RESET");
            _animator.Advance(0);
            _animator.Play("RotateClockwise");
        }

        private Node2D GetPipeDisplay(CardinalDirection dir)
        {
            return GetNode<Node2D>($"%PipeDisplays/{dir}");
        }
    }
}
