using Godot;

namespace RandomDungeons
{
    public class InvulnerableFlasher : Node
    {
        public const float FlashesPerSecond = 15;
        private float _flashInterval => 1 / (FlashesPerSecond * 2);

        [Export] public NodePath VisualsPath;
        [Export] public NodePath HurtBoxPath;

        private Node2D _visuals => GetNode<Node2D>(VisualsPath);
        private HurtBox _hurtBox => GetNode<HurtBox>(HurtBoxPath);
        private float _timer = 0;

        public override void _Process(float delta)
        {
            _timer += delta;
            if (_timer > _flashInterval)
            {
                _visuals.Visible = !_visuals.Visible;
                _timer -= _flashInterval;
            }

            if (!_hurtBox.IsInvulnerable)
                _visuals.Visible = true;
        }
    }
}
