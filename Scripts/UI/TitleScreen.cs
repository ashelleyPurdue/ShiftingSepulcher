using Godot;

namespace RandomDungeons
{
    public class TitleScreen : Control
    {
        public static int ChosenSeed;

        [Export] public PackedScene DungeonModeScene;
        [Export] public PackedScene LightsOutScene;
        [Export] public PackedScene SlidingIceModeScene;

        private SeedInput _seedInput => GetNode<SeedInput>("%SeedInput");


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
            ChosenSeed = _seedInput.ParseSeedTextbox();
            GetTree().ChangeSceneTo(modeScene);
        }
    }
}


