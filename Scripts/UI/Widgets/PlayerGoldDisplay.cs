using Godot;

namespace ShiftingSepulcher
{
    public class PlayerGoldDisplay : Label
    {
        public override void _Process(float delta)
        {
            Text = $"Gold: {PlayerInventory.Gold}";
        }
    }
}
