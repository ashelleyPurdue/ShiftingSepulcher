using Godot;

namespace RandomDungeons
{
    public class NodeEntity2D : Node2D, IEntity<Node2D>
    {
        Node2D IEntity<Node2D>.Node => this;

        public override void _Ready()
        {
            this.SendEntityReady();
        }
    }
}
