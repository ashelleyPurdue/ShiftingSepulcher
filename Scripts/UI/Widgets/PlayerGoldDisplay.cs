using Godot;

namespace RandomDungeons
{
    public class PlayerGoldDisplay : Label
    {
        public override void _Process(float delta)
        {
            Text = $"Gold: {PlayerInventory.Gold}";
        }
    }
}
