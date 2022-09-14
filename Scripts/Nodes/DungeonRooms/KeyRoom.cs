using Godot;
using RandomDungeons.Graphs;
using RandomDungeons.Nodes.Elements;

namespace RandomDungeons.Nodes.DungeonRooms
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
