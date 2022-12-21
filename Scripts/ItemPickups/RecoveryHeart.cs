using Godot;

namespace RandomDungeons
{
    public class RecoveryHeart : Node
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
