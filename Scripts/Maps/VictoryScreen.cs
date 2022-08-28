using Godot;

namespace RandomDungeons
{
    public class VictoryScreen : Node
    {
        private void BackToTitleButtonPressed()
        {
            GetTree().ChangeScene("res://Maps/TitleScreen.tscn");
        }
    }
}
