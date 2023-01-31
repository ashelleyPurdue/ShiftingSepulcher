using Godot;

namespace RandomDungeons
{
    public class TitleScreen : Control
    {
        public static int ChosenSeed;

        [Export] public NodePath DefaultFocusedOption;

        [Export] public PackedScene DungeonModeScene;

        [Export] public bool AllowSkipIntro = true;

        private Control _options => GetNode<Control>("%Options");
        private Control _defaultFocusedOption => GetNode<Control>(DefaultFocusedOption);

        private AnimationPlayer _animationPlayer => GetNode<AnimationPlayer>("%AnimationPlayer");
        private SeedInput _seedInput => GetNode<SeedInput>("%SeedInput");

        public override void _Ready()
        {
            MusicService.Instance.StopSong();
        }

        public override void _Input(InputEvent ev)
        {
            if (ev.IsPressed() && AllowSkipIntro)
                SkipIntro();
        }

        public void SkipIntro()
        {
            _animationPlayer.ResetAndPlay("SkippedIntro");
        }

        private void PlayDungeonMode()
        {
            ChosenSeed = _seedInput.ParseSeedTextbox();
            GetTree().ChangeSceneTo(DungeonModeScene);
        }
    }
}


