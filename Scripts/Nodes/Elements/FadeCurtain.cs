using Godot;

namespace RandomDungeons.Nodes.Elements
{
    public class FadeCurtain : Node2D
    {
        public float FadePercent;

        public override void _Process(float delta)
        {
            Visible = FadePercent > 0;
            Modulate = GetBackgroundColor(1 - FadePercent);
        }

        private Color GetBackgroundColor(float alpha)
        {
            var c = (Color)ProjectSettings.GetSetting("rendering/environment/default_clear_color");
            c.a = alpha;

            return c;
        }
    }
}