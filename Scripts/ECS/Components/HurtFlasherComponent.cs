using Godot;

namespace RandomDungeons
{
    public class HurtFlasherComponent : BaseComponent<Node2D>
    {
        [Export] public float FlashDuration = 0.25f;
        [Export] public Color FlashColor = Colors.Red;

        private Color _originalColor;
        private float _timer = 0;
        private bool _isFlashing = false;

        private HealthPointsComponent _healthPoints => this.GetComponent<HealthPointsComponent>();

        public override void _EntityReady()
        {
            _healthPoints.Connect(
                signal: nameof(HealthPointsComponent.TookDamageNoParams),
                target: this,
                method: nameof(OnTookDamage)
            );
        }

        public override void _Process(float delta)
        {
            if (!_isFlashing)
                return;

            _timer -= delta;

            Entity.Modulate = _originalColor.LinearInterpolate(
                FlashColor,
                _timer / FlashDuration
            );

            if (_timer <= 0)
            {
                Entity.Modulate = _originalColor;
                _isFlashing = false;
            }
        }

        private void OnTookDamage()
        {
            Flash();

            if (_healthPoints.Health <= 0)
                Cancel();
        }

        private void Flash()
        {
            if (!_isFlashing)
            {
                _originalColor = Entity.Modulate;
                _isFlashing = true;
            }

            Entity.Modulate = FlashColor;
            _timer = FlashDuration;
        }

        private void Cancel()
        {
            if (!_isFlashing)
                return;

            Entity.Modulate = _originalColor;
            _isFlashing = false;
        }
    }
}
