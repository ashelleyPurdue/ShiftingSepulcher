using Godot;

namespace RandomDungeons.Nodes.Maps
{
    public class TitleScreen : Control
    {
        public static int ChosenSeed;

        [Export] public PackedScene DungeonModeScene;
        [Export] public PackedScene LightsOutScene;
        [Export] public PackedScene SlidingIceModeScene;

        private LineEdit _seedTextBox => GetNode<LineEdit>("%SeedTextBox");

        public override void _Ready()
        {
            RandomizeSeed();
        }

        public void RandomizeSeed()
        {
            GD.Randomize();
            _seedTextBox.Text = "" + GD.Randi();
        }

        private void PlayDungeonMode()
        {
            ChooseMode(DungeonModeScene);
        }

        private void PlayLightsOutMode()
        {
            ChooseMode(LightsOutScene);
        }

        private void PlaySlidingIceMode()
        {
            ChooseMode(SlidingIceModeScene);
        }

        private void ChooseMode(PackedScene modeScene)
        {
            ChosenSeed = ParseSeedTextbox();
            GetTree().ChangeSceneTo(modeScene);
        }

        private int ParseSeedTextbox()
        {
            bool isNumber = int.TryParse(_seedTextBox.Text, out int seed);
            if (!isNumber)
                seed = (int)_seedTextBox.Text.Hash();

            return seed;
        }

    }
}


