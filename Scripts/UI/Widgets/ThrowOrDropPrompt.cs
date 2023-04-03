using Godot;

namespace ShiftingSepulcher
{
    public class ThrowOrDropPrompt : Node2D
    {
        private AnimationPlayer _animator => GetNode<AnimationPlayer>("%AnimationPlayer");
        private bool _wasHolding = false;

        public override void _Ready()
        {
            // Hide the prompt until the first time the player picks something
            // up.  It will then remain "visible" forever, although the animation
            // will shrink it to size 0 when the player drops it.
            Visible = false;
        }

        public override void _Process(float delta)
        {
            var player = GetTree().FindPlayer();

            if (player.IsHoldingSomething && !_wasHolding)
            {
                _animator.ResetAndPlay("Appear");
                Visible = true;
            }

            if (!player.IsHoldingSomething && _wasHolding)
                _animator.PlayBackwards("Appear");

            _wasHolding = player.IsHoldingSomething;
        }
    }
}
