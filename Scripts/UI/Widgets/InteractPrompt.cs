using Godot;

namespace ShiftingSepulcher
{
    public class InteractPrompt : Node2D
    {
        private AnimationPlayer _animator => GetNode<AnimationPlayer>("%AnimationPlayer");
        private Label _promptTextLabel => GetNode<Label>("%PromptTextLabel");

        private bool _wasVisible = false;
        private Vector2 _pos;
        private Vector2 _targetPos;

        public override void _Process(float delta)
        {
            var player = GetTree().FindPlayer();
            var highlightedObject = player.HighlightedObject;

            Visible = player.IsObjectHighlighted && !player.IsHoldingSomething;

            if (Visible)
            {
                _promptTextLabel.Text = highlightedObject.PromptText;
                _targetPos = highlightedObject.PromptGlobalPosition;
            }

            // Play the animation when first appearing
            if (Visible && !_wasVisible)
            {
                _animator.ResetAndPlay("Appear");
                _pos = _targetPos;
            }
            _wasVisible = Visible;

            // Glide to the appropriate position if the player changed targets
            // without the prompt disappearing
            float deltaPos = 32 * 60 * 0.25f * delta;
            _pos = _pos.MoveToward(_targetPos, deltaPos);

            GlobalPosition = _pos;
        }
    }
}
