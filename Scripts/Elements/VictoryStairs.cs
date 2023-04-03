using Godot;

namespace ShiftingSepulcher
{
    public class VictoryStairs : Node2D
    {
        private void TriggerBodyEntered(object body)
        {
            if (body is Player)
            {
                GetTree().ChangeScene("res://Scenes/Maps/VictoryScreen.tscn");
            }
        }
    }
}
