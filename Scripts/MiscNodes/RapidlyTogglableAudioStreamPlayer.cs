using Godot;

namespace ShiftingSepulcher
{
    /// <summary>
    /// Provides a workaround for a Godot bug that causes the audio to not stop
    /// if Playing is set to false on the same frame it was set to true.
    /// See https://github.com/godotengine/godot/issues/37148
    /// </summary>
    public class RapidlyTogglableAudioStreamPlayer : AudioStreamPlayer
    {
        [Export] public bool PlayingTogglable;

        public override void _Process(float delta)
        {
            if (Playing != PlayingTogglable)
                Playing = PlayingTogglable;
        }
    }
}
