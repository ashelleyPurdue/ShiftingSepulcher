using Godot;

namespace RandomDungeons.Nodes.Elements
{
    public class FadeCurtain : Node2D
    {
        private const float FadeTime = 0.25f;

        public bool DoneFadingOut => FadePercent <= 0;

        public float FadePercent;
        private float _targetFadePercent;

        public override void _Process(float delta)
        {
            FadePercent = Mathf.MoveToward(
                FadePercent,
                _targetFadePercent,
                delta / FadeTime
            );

            Visible = FadePercent > 0;
            Modulate = GetBackgroundColor(1 - FadePercent);
        }

        public void FadeIn()
        {
            _targetFadePercent = 1;
        }

        public void FadeOut()
        {
            _targetFadePercent = 0;
        }

        private Color GetBackgroundColor(float alpha)
        {
            var c = (Color)ProjectSettings.GetSetting("rendering/environment/default_clear_color");
            c.a = alpha;

            return c;
        }
    }
}