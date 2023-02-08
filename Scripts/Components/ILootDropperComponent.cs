using Godot;

namespace RandomDungeons
{
    public interface ILootDropperComponent : IComponent<Node2D>
    {
        void DropLoot();
    }
}
