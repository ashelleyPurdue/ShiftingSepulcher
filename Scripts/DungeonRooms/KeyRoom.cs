using Godot;

namespace RandomDungeons
{
    public class KeyRoom : SimpleDungeonRoom
    {
        private Key _key => GetNode<Key>("%Key");

        public override void Populate(DungeonLayoutRoom layoutRoom)
        {
            base.Populate(layoutRoom);
            _key.KeyId = layoutRoom.TreeRoom.KeyId;
        }
    }
}
