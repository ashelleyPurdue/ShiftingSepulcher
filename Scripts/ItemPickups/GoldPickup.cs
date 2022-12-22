using Godot;

namespace RandomDungeons
{
    public class GoldPickup : Node2D
    {
        [Export] public int GoldValue;

        public void OnPickedUp()
        {
            PlayerInventory.Gold += GoldValue;
            QueueFree();
        }
    }
}
