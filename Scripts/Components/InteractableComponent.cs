using Godot;

namespace RandomDungeons
{
    [CustomNode]
    public class InteractableComponent : BaseComponent<Node2D>
    {
        [Signal] public delegate void Interacted();

        [Export] public string PromptText {get; set;} = "";
        [Export] public NodePath PromptPositionPath;

        public Vector2 PromptGlobalPosition => PromptPositionPath != null
            ? GetNode<Node2D>(PromptPositionPath).GlobalPosition
            : Entity.GlobalPosition;

        public void Interact()
        {
            EmitSignal(nameof(Interacted));
        }
    }
}
