using Godot;

namespace ShiftingSepulcher
{
    public class RecoveryHeart : Node2D, IRespawnable
    {
        public void OnPickedUp()
        {
            var hp = GetTree().FindPlayer().HealthPoints;
            hp.Health++;

            if (hp.Health > hp.MaxHealth)
                hp.Health = hp.MaxHealth;

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
