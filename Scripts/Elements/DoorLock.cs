using Godot;

namespace ShiftingSepulcher
{
    public class DoorLock : Node2D
    {
        [Export] public int KeyId;
        private bool _isUnlocked = false;

        public override void _Process(float delta)
        {
            Modulate = KeyColors.ForId(KeyId);
        }

        public void OnUnlockTriggerBodyEnter(object body)
        {
            if (body is Player && PlayerInventory.HasKey(KeyId) && !_isUnlocked)
            {
                PlayerInventory.SpendKey(KeyId);
                _isUnlocked = true;
                GetNode<AnimationPlayer>("%AnimationPlayer").Play("Unlock");
            }
        }
    }
}
