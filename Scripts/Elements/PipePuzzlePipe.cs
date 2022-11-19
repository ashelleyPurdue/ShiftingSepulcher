using Godot;

namespace RandomDungeons
{
    public class PipePuzzlePipe : Node2D
    {
        public PipePuzzleGraph.Cell Cell;

        public override void _Process(float delta)
        {
            foreach (var dir in CardinalDirectionUtils.All())
            {
                GetPipeDisplay(dir).Visible = Cell.IsDirectionOpen(dir);
            }
        }

        private Node2D GetPipeDisplay(CardinalDirection dir)
        {
            return GetNode<Node2D>($"%PipeDisplays/{dir}");
        }
    }
}
