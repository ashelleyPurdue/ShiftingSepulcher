using Godot;

namespace ShiftingSepulcher
{
    public interface ILootDropperComponent : IComponent<Node2D>
    {
        void DropLoot();
    }
}
