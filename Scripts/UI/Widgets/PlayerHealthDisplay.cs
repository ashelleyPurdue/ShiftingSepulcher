using Godot;

namespace RandomDungeons
{
    [Tool]
    public class PlayerHealthDisplay : Control
    {
        [Export] public Texture Image;
        [Export] public Vector2 ImageRectSize = new Vector2(32, 32);

        private HealthPointsComponent _hp;

        public override void _Draw()
        {
            if (Image == null)
                return;

            int health = _hp?.Health ?? 6;

            for (int i = 0; i < health; i++)
            {
                var offset = new Vector2(ImageRectSize.x * i, 0);

                DrawTextureRect(
                    texture: Image,
                    rect: new Rect2(
                        offset,
                        ImageRectSize
                    ),
                    tile: false
                );
            }
        }

        public override void _Process(float delta)
        {
            if (!Engine.EditorHint && _hp == null)
            {
                _hp = GetTree().FindPlayer()?.HealthPoints;
            }

            Update();
        }
    }
}
