using Godot;

namespace RandomDungeons
{
    public class PauseMenu : Control
    {
        private bool _isOpen = false;

        public override void _Process(float delta)
        {
            Visible = _isOpen;

            if (InputService.PausePressed)
            {
                if (!_isOpen)
                    Pause();
                else
                    Unpause();
            }
        }

        public void Pause()
        {
            _isOpen = true;
            GetTree().Paused = true;
        }

        public void Unpause()
        {
            _isOpen = false;
            GetTree().Paused = false;
        }

        public void ReturnToTitleScreen()
        {
            Unpause();
            GetTree().ChangeScene("res://Scenes/Maps/TitleScreen.tscn");
        }
    }
}
