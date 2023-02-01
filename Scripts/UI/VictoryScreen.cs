using Godot;

namespace RandomDungeons
{
    public class VictoryScreen : Node
    {
        private Button _backToTitleButton => GetNode<Button>("%BackToTitleButton");

        public override void _Ready()
        {
            MusicService.Instance.StopSong();
            _backToTitleButton.GrabFocus();
        }

        private void BackToTitleButtonPressed()
        {
            GetTree().ChangeScene("res://Scenes/Maps/TitleScreen.tscn");
        }
    }
}
