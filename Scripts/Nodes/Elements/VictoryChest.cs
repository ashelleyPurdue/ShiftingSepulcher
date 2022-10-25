using Godot;

namespace RandomDungeons
{
    public class VictoryChest : Node2D
    {
        private void OpenChestTriggerBodyEntered(object body)
        {
            if (body is Player)
            {
                GetTree().ChangeScene("res://Scenes/Maps/VictoryScreen.tscn");
            }
        }
    }
}
