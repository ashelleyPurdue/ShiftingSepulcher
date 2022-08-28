using Godot;

namespace RandomDungeons.Nodes.Elements
{
    public class VictoryChest : Node2D
    {
        private void OpenChestTriggerBodyEntered(object body)
        {
            if (body is Player)
            {
                GetTree().ChangeScene("res://Maps/VictoryScreen.tscn");
            }
        }
    }
}
