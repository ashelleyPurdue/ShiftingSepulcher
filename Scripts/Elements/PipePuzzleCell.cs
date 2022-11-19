using Godot;

namespace RandomDungeons
{
    public class PipePuzzleCell : Node2D
    {
        public PipePuzzleGraph.Cell Cell;

        private AnimationPlayer _animator => GetNode<AnimationPlayer>("%Animator");

        public override void _Process(float delta)
        {
            foreach (var dir in CardinalDirectionUtils.All())
            {
                GetPipeDisplay(dir).Visible = Cell.IsDirectionOpen(dir);
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
