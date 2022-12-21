using Godot;

namespace RandomDungeons
{
    public class RecoveryHeart : Node2D
    {
        public void OnPickedUp()
        {
            PlayerInventory.Health++;

            if (PlayerInventory.Health > PlayerInventory.MaxHealth)
                PlayerInventory.Health = PlayerInventory.MaxHealth;

            QueueFree();
        }
    }
}
