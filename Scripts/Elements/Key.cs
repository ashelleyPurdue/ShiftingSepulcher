using Godot;

namespace ShiftingSepulcher
{
    public class Key : Node2D
    {
        [Export] public int KeyId;

        private bool _opened = false;

        public override void _Ready()
        {
            // // Color the key according to its id
            // Modulate = KeyColors.ForId(KeyId);
        }

        public void OnInteracted()
        {
            if (_opened)
                return;

            // TODO: Play the opening animation
            GetNode<ChestModel>("%ChestModel").OpenPercent = 1;

            _opened = true;
            PlayerInventory.CollectKey(KeyId);
            GetNode<LingeringAudioStreamPlayer>("%CollectSound").Play();
        }
    }
}



