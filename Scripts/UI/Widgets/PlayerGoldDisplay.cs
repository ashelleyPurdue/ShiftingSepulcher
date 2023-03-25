using Godot;

namespace ShiftingSepulcher
{
    public class PlayerGoldDisplay : Label
    {
        private int _displayedGold = 0;

        public override void _Ready()
        {
            _displayedGold = PlayerInventory.Gold;
        }

        public override void _Process(float delta)
        {
            Text = $"Gold: {PlayerInventory.Gold}";
        }

        public override void _PhysicsProcess(float delta)
        {
            if (_displayedGold < PlayerInventory.Gold)
                _displayedGold++;
        }
    }
}
