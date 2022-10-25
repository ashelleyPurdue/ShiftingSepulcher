using Godot;

namespace RandomDungeons
{
    public class KeyRoom : SimpleDungeonRoom
    {
        private Key _key => GetNode<Key>("%Key");

        public override void Populate(DungeonGraphRoom graphRoom)
        {
            base.Populate(graphRoom);
            _key.KeyId = graphRoom.KeyId;
        }
    }
}
