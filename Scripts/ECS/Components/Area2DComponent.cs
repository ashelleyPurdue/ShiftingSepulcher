using Godot;

namespace RandomDungeons
{
    public class Area2DComponent : Area2D, IComponent<Node2D>
    {
        public Node2D Entity => GetParent<Node2D>();
        Node IComponent.Entity => GetParent();

        public void _EntityReady() {}
    }
}
