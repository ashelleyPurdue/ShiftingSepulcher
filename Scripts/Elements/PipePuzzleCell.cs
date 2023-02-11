using Godot;

namespace ShiftingSepulcher
{
    public class PipePuzzleCell : Node2D
    {
        [Export] public Color UnpoweredColor;
        [Export] public Color PoweredColor;

        public event System.Action CellRotated;

        public PipePuzzleGraph.Cell Cell;
        public bool IsPowered;

        private AnimationPlayer _animator => GetNode<AnimationPlayer>("%Animator");

        public override void _EnterTree()
        {
            UpdatePipeDisplays();
        }

        public void UpdatePipeDisplays()
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
            CellRotated?.Invoke();

            _animator.ResetAndPlay("RotateClockwise");
        }

        private Node2D GetPipeDisplay(CardinalDirection dir)
        {
            return GetNode<Node2D>($"%PipeDisplays/{dir}");
        }
    }
}
