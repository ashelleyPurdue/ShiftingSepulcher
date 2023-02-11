using Godot;

namespace ShiftingSepulcher
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
