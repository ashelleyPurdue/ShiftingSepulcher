using Godot;

namespace RandomDungeons
{
    public class InteractPrompt : Node2D
    {
        private Label _promptTextLabel => GetNode<Label>("%PromptTextLabel");

        public override void _Process(float delta)
        {
            var player = GetTree().FindPlayer();
            var highlightedObject = player.HighlightedObject;

            if (highlightedObject == null || player.IsHoldingSomething)
            {
                Visible = false;
                return;
            }

            Visible = true;
            _promptTextLabel.Text = highlightedObject.PromptText;
            GlobalPosition = highlightedObject.PromptGlobalPosition;
        }
    }
}
