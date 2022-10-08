using Godot;

namespace RandomDungeons.Nodes.UI
{
    public class VictoryScreen : Node
    {
        private void BackToTitleButtonPressed()
        {
            GetTree().ChangeScene("res://Scenes/Maps/TitleScreen.tscn");
        }
    }
}
