using Godot;

namespace RandomDungeons
{
    public class TitleScreen : Control
    {
        public static int ChosenSeed;

        [Export] public NodePath DefaultFocusedOption;

        [Export] public PackedScene DungeonModeScene;

        private Control _defaultFocusedOption => GetNode<Control>(DefaultFocusedOption);
        private SeedInput _seedInput => GetNode<SeedInput>("%SeedInput");

        public override void _Ready()
        {
            _defaultFocusedOption.GrabFocus();
            MusicService.Instance.StopSong();
        }

        private void PlayDungeonMode()
        {
            ChosenSeed = _seedInput.ParseSeedTextbox();
            GetTree().ChangeSceneTo(DungeonModeScene);
        }
    }
}


