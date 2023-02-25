using Godot;

namespace ShiftingSepulcher
{
    public class Key : Node2D
    {
        [Export] public int KeyId;

        public override void _Ready()
        {
            // Color the key according to its id
            GetNode<Node2D>("%KeySprite").Modulate = KeyColors.ForId(KeyId);
        }

        public void OnInteracted()
        {
            this.GetComponent<InteractableComponent>().InteractEnabled = false;
            PlayerInventory.CollectKey(KeyId);

            GetNode<AnimationPlayer>("%AnimationPlayer").ResetAndPlay("Open");
        }
    }
}



