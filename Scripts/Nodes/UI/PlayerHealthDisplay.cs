using Godot;
using RandomDungeons.Services;

namespace RandomDungeons.Nodes.UI
{
    public class PlayerHealthDisplay : Control
    {
        [Export] public Texture Image;
        [Export] public Vector2 ImageRectSize = new Vector2(32, 32);

        public override void _Draw()
        {
            for (int i = 0; i < PlayerInventory.Health; i++)
            {
                DrawTextureRect(
                    texture: Image,
                    rect: new Rect2(
                        new Vector2(ImageRectSize.x * i, 0),
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
