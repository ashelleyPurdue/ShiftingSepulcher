using Godot;

namespace RandomDungeons.Nodes.Elements
{
    public class FadeCurtain : Node2D
    {
        private const float FadeTime = 0.25f;

        public bool DoneFadingOut => _alpha >= 1;

        public float _alpha;
        private float _targetAlpha;

        public override void _Process(float delta)
        {
            _alpha = Mathf.MoveToward(
                _alpha,
                _targetAlpha,
                delta / FadeTime
            );

            Visible = _alpha > 0;
            Modulate = GetBackgroundColor(_alpha);
        }

        public void FadeIn()
        {
            _alpha = 1;
            _targetAlpha = 0;
        }

        public void FadeOut()
        {
            _alpha = 0;
            _targetAlpha = 1;
        }

        private Color GetBackgroundColor(float alpha)
        {
            var c = (Color)ProjectSettings.GetSetting("rendering/environment/default_clear_color");
            c.a = alpha;

            return c;
        }
    }
}
