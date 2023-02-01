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

            Label("%ClearTimeDisplay").Text = FormatTime(PlayerInventory.ClearTime);
            Label("%GoldCount").Text = PlayerInventory.Gold.ToString();
        }

        private void BackToTitleButtonPressed()
        {
            GetTree().ChangeScene("res://Scenes/Maps/TitleScreen.tscn");
        }

        private Label Label(string nodePath) => GetNode<Label>(nodePath);

        private string FormatTime(float totalSeconds)
        {
            int minutes = (int)(totalSeconds / 60);
            int seconds = (int)(totalSeconds - (minutes * 60));

            return $"{minutes}:{seconds.ToString("D2")}";
        }
    }
}
