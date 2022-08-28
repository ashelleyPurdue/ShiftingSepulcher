using Godot;

namespace RandomDungeons.Nodes.Maps
{
    public class VictoryScreen : Node
    {
        private void BackToTitleButtonPressed()
        {
            GetTree().ChangeScene("res://Maps/TitleScreen.tscn");
        }
    }
}
