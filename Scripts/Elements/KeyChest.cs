using Godot;

namespace ShiftingSepulcher
{
    public class KeyChest : Node2D
    {
        [Export] public int KeyId;
        private bool _opened = false;

        private AnimationPlayer _animator => GetNode<AnimationPlayer>("%AnimationPlayer");

        public override void _ExitTree()
        {
            // Skip to the end of the opening animation if the player leaves the
            // room.  That way, the player won't see the item still floating
            // there when he comes back
            if (_opened)
                _animator.ResetAndPlay("SkippedOpen");
        }

        public void OnInteracted()
        {
            _opened = true;
            this.GetComponent<InteractableComponent>().InteractEnabled = false;
            PlayerInventory.CollectKey(KeyId);

           _animator.ResetAndPlay("Open");

            // Color the key according to its id
            GetNode<Node2D>("%KeySprite").Modulate = KeyColors.ForId(KeyId);
        }
    }
}



