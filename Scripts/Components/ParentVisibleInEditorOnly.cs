using Godot;

namespace RandomDungeons
{
    [CustomNode]
    public class ParentVisibleInEditorOnly : Node
    {
        public override void _Ready()
        {
            GetParent<Node2D>().Visible = false;
        }
    }
}
