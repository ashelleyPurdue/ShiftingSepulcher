using Godot;

namespace RandomDungeons
{
    public class TitleScreen : Control
    {
        public static int ChosenSeed;

        [Export] public NodePath DefaultFocusedOption;

        [Export] public PackedScene DungeonModeScene;
        [Export] public PackedScene SlidingIceModeScene;
        [Export] public PackedScene ScalePuzzleScene;

        private Control _defaultFocusedOption => GetNode<Control>(DefaultFocusedOption);
        private SeedInput _seedInput => GetNode<SeedInput>("%SeedInput");

        public override void _Ready()
        {
            _defaultFocusedOption.GrabFocus();
            MusicService.Instance.StopSong();
        }

        private void PlayDungeonMode()
        {
            ChooseMode(DungeonModeScene);
        }

        private void PlaySlidingIceMode()
        {
            ChooseMode(SlidingIceModeScene);
        }

        private void PlayScalePuzzleMode()
        {
            ChooseMode(ScalePuzzleScene);
        }

        private void ChooseMode(PackedScene modeScene)
        {
            ChosenSeed = _seedInput.ParseSeedTextbox();
            GetTree().ChangeSceneTo(modeScene);
        }
    }
}


