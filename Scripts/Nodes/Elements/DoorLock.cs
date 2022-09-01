using Godot;
using RandomDungeons.Services;
using RandomDungeons.Utils;

namespace RandomDungeons.Nodes.Elements
{
    public class DoorLock : Node2D
    {
        [Export] public int KeyId;

        public override void _Process(float delta)
        {
            Modulate = KeyColors.ForId(KeyId);
        }

        public void OnUnlockTriggerBodyEnter(object body)
        {
            if (body is Player && PlayerInventory.HasKey(KeyId))
            {
                PlayerInventory.SpendKey(KeyId);
                QueueFree();
            }
        }
    }
}
