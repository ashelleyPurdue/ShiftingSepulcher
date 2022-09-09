using Godot;

namespace RandomDungeons.Nodes.Components
{
    public class HurtFlasher : Node
    {
        [Export] public NodePath VisualsPath;
        [Export] public float FlashDuration = 0.25f;
        [Export] public Color FlashColor = Colors.Red;

        private Node2D _visuals => GetNode<Node2D>(VisualsPath);

        private Color _originalColor;
        private float _timer = 0;
        private bool _isFlashing = false;

        public override void _Process(float delta)
        {
            if (!_isFlashing)
                return;

            _timer -= delta;

            _visuals.Modulate = _originalColor.LinearInterpolate(
                FlashColor,
                _timer / FlashDuration
            );

            if (_timer <= 0)
            {
                _visuals.Modulate = _originalColor;
                _isFlashing = false;
            }
        }

        public void Flash()
        {
            if (!_isFlashing)
            {
                _originalColor = _visuals.Modulate;
                _isFlashing = true;
            }

            _visuals.Modulate = FlashColor;
            _timer = FlashDuration;
        }
    }
}
