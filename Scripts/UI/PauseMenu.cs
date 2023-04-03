using Godot;

namespace ShiftingSepulcher
{
    public class PauseMenu : Control
    {
        [Export] public NodePath DefaultFocusedOption;

        private Control _defaultFocusedOption => GetNode<Control>(DefaultFocusedOption);

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
            _defaultFocusedOption.GrabFocus();
            GetTree().Paused = true;
        }

        public void Unpause()
        {
            _isOpen = false;
            GetTree().Paused = false;

            GetNode<Popup>("%SettingsMenu").Visible = false;
        }

        public void ReturnToTitleScreen()
        {
            Unpause();
            GetTree().ChangeScene("res://Scenes/Maps/TitleScreen.tscn");
        }
    }
}
