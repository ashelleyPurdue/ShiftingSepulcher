using Godot;

namespace ShiftingSepulcher
{
    public class Shockwave : HitBox
    {
        [Export] public float StartRadius = 16;
        [Export] public float EndRadius = 32;
        [Export] public float ActiveDuration = 0.2f;
        [Export] public float FadeDuration = 0.25f;

        private bool _isGrowing = true;
        private float _timer = 0;

        public override void _PhysicsProcess(float delta)
        {
            _timer += delta;

            if (_isGrowing)
                WhileGrowing();
            else
                WhileFading();
        }

        private void WhileGrowing()
        {
            float t = _timer / ActiveDuration;
            SetRadius(Mathf.Lerp(StartRadius, EndRadius, t));

            if (_timer >= ActiveDuration)
            {
                SetRadius(EndRadius);

                _timer -= ActiveDuration;
                _isGrowing = false;
                Enabled = false;
            }
        }

        private void WhileFading()
        {
            var targetColor = Colors.White;
            targetColor.a = 0;

            float t = _timer / FadeDuration;
            Modulate = Colors.White.LinearInterpolate(targetColor, t);

            if (_timer >= FadeDuration)
                QueueFree();
        }

        private void SetRadius(float radius)
        {
            Scale = Vector2.One * radius / 16;
        }
    }
}
