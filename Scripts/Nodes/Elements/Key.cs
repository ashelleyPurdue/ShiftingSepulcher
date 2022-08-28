using Godot;

using RandomDungeons.Services;
using RandomDungeons.Utils;

namespace RandomDungeons.Nodes.Elements
{
    public class Key : Node2D
    {
        [Export] public int KeyId;

        public override void _Ready()
        {
            // Color the key according to its id
            Modulate = KeyColors.ForId(KeyId);
        }

        private void BodyEntered(object body)
        {
            if (body is Player)
            {
                PlayerInventory.CollectKey(KeyId);
                QueueFree();
            }
        }
    }
}



