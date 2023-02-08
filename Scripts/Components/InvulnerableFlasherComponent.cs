using Godot;

namespace RandomDungeons
{
    [CustomNode]
    public class InvulnerableFlasherComponent : BaseComponent<Node2D>
    {
        public const float FlashesPerSecond = 15;
        private HealthPointsComponent _hp;

        private float _flashInterval => 1 / (FlashesPerSecond * 2);
        private float _timer = 0;

        public override void _EntityReady()
        {
            _hp = this.GetComponent<HealthPointsComponent>();
        }

        public override void _Process(float delta)
        {
            _timer += delta;
            if (_timer > _flashInterval)
            {
                Entity.Visible = !Entity.Visible;
                _timer -= _flashInterval;
            }

            if (!_hp.IsInvulnerable)
                Entity.Visible = true;
        }
    }
}
