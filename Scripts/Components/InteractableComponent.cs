using Godot;

namespace ShiftingSepulcher
{
    [CustomNode]
    public class InteractableComponent : BaseComponent<Node2D>
    {
        [Signal] public delegate void Interacted();

        [Export] public string PromptText {get; set;} = "";
        [Export] public NodePath PromptPositionPath;

        [Export] public bool InteractEnabled = true;

        public Vector2 PromptGlobalPosition => PromptPositionPath != null
            ? GetNode<Node2D>(PromptPositionPath).GlobalPosition
            : Entity.GlobalPosition;

        public void Interact()
        {
            if (InteractEnabled)
                EmitSignal(nameof(Interacted));
        }
    }
}
