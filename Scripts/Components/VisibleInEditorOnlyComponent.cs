using Godot;

namespace ShiftingSepulcher
{
    [CustomNode]
    public class VisibleInEditorOnlyComponent : Node
    {
        public override void _Ready()
        {
            GetParent<Node2D>().Visible = false;
        }
    }
}
