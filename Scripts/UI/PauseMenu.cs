using Godot;

namespace RandomDungeons
{
    public class PauseMenu : Control
    {
        private bool _isOpen = false;

        public override void _Process(float delta)
        {
            Visible = _isOpen;
            GetTree().Paused = _isOpen;

            if (InputService.PausePressed)
                _isOpen = !_isOpen;
        }
    }
}
