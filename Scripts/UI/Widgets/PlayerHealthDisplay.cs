using Godot;

namespace RandomDungeons
{
    [Tool]
    public class PlayerHealthDisplay : Control
    {
        [Export] public Texture Image;
        [Export] public Vector2 ImageRectSize = new Vector2(32, 32);

        public override void _Draw()
        {
            if (Image == null)
                return;

            int health = Engine.EditorHint
                ? 6
                : PlayerInventory.Health;

            for (int i = 0; i < health; i++)
            {
                var offset = new Vector2(ImageRectSize.x * i, 0);

                DrawTextureRect(
                    texture: Image,
                    rect: new Rect2(
                        RectPosition + offset,
                        ImageRectSize
                    ),
                    tile: false
                );
            }
        }

        public override void _Process(float delta)
        {
            Update();
        }
    }
}
