using Godot;

namespace ShiftingSepulcher
{
    public class KeyRoom : SimpleDungeonRoom
    {
        private KeyChest _key => GetNode<KeyChest>("%Key");

        public override void Populate(DungeonLayoutRoom layoutRoom)
        {
            base.Populate(layoutRoom);
            _key.KeyId = layoutRoom.TreeRoom.KeyId;
        }
    }
}
