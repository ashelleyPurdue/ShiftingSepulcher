using Godot;

namespace ShiftingSepulcher
{
    public class PlayerGoldDisplay : Label
    {
        [Export] public float PlingSoundMinLength = 0.05f;

        private int _displayedGold = 0;

        public override void _Ready()
        {
            _displayedGold = PlayerInventory.Gold;
        }

        public override void _Process(float delta)
        {
            Text = $"Gold: {_displayedGold}";
        }

        public override void _PhysicsProcess(float delta)
        {
            if (_displayedGold < PlayerInventory.Gold)
            {
                _displayedGold++;

                var plingSound = GetNode<AudioStreamPlayer>("%PlingSound");
                float soundPos = plingSound.GetPlaybackPosition();

                if (!plingSound.Playing || soundPos > PlingSoundMinLength)
                    plingSound.Play();
            }
        }
    }
}
