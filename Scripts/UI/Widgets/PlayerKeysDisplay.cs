using System.Linq;
using Godot;

namespace ShiftingSepulcher
{
    [Tool]
    public class PlayerKeysDisplay : Control
    {
        [Export] public Texture Image;
        [Export] public Vector2 ImageRectSize = new Vector2(32, 32);

        public override void _Draw()
        {
            if (Image == null)
                return;

            int[] heldKeys = Engine.EditorHint
                ? new[] {1, 2, 3, 4}
                : PlayerInventory.HeldKeys.ToArray();


            for (int i = 0; i < heldKeys.Length; i++)
            {
                var offset = new Vector2(ImageRectSize.x * i, 0);

                DrawTextureRect(
                    texture: Image,
                    rect: new Rect2(
                        offset,
                        ImageRectSize
                    ),
                    tile: false,
                    modulate: KeyColors.ForId(heldKeys[i])
                );
            }
        }

        public override void _Process(float delta)
        {
            Update();
        }
    }
}
