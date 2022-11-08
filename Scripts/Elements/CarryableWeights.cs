using Godot;

namespace RandomDungeons
{
    public class CarryableWeights : Node2D
    {
        [Export] public int NumWeights = 1;
        [Export] public float WeightVerticalSeparation = 12;

        private Node2D _weightVisualTemplate => GetNode<Node2D>("%WeightVisualTemplate");
        private Node2D _weightStack => GetNode<Node2D>("%WeightStack");

        public override void _Ready()
        {
            _weightVisualTemplate.Visible = false;
        }

        public override void _Process(float delta)
        {
            // Make sure there's the right amount of weights
            while (_weightStack.GetChildCount() < NumWeights)
            {
                var weightClone = (Node2D)_weightVisualTemplate.Duplicate((int)DuplicateFlags.UseInstancing);
                weightClone.Visible = true;

                _weightStack.AddChild(weightClone);
            }

            while (_weightStack.GetChildCount() > NumWeights)
            {
                _weightStack.RemoveChild(_weightStack.GetChild(0));
            }

            // Make sure the weights are all vertically separated
            for (int i = 0; i < NumWeights; i++)
            {
                var weight = _weightStack.GetChild<Node2D>(i);
                weight.Position = Vector2.Up * i * WeightVerticalSeparation;
            }
        }
    }
}
