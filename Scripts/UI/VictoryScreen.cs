using Godot;

namespace RandomDungeons
{
    public class VictoryScreen : Node
    {
        public override void _Ready()
        {
            MusicService.Instance.StopSong();
        }

        private void BackToTitleButtonPressed()
        {
            GetTree().ChangeScene("res://Scenes/Maps/TitleScreen.tscn");
        }
    }
}
