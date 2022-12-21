using Godot;

namespace RandomDungeons
{
    public class RecoveryHeart : Node2D, IRespawnable
    {
        public void OnPickedUp()
        {
            PlayerInventory.Health++;

            if (PlayerInventory.Health > PlayerInventory.MaxHealth)
                PlayerInventory.Health = PlayerInventory.MaxHealth;

            QueueFree();
        }

        public void Respawn()
        {
            // Despawn the heart whenever the player dies.
            // This way, the player can't keep suiciding and re-killing enemies
            // to create a ton of hearts on the ground.
            QueueFree();
        }
    }
}
