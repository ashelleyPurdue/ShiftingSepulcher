using Godot;

namespace ShiftingSepulcher
{
    /// <summary>
    /// A wrapper around <see cref="MusicService"/> that can be accessed from
    /// an animation.  It can also be used as a handler for signals.
    /// </summary>
    public class MusicServiceAccessor : Node
    {
        public void PlaySong(BackgroundMusicSong song) => MusicService.Instance.PlaySong(song);
        public void StopSong() => MusicService.Instance.StopSong();
    }
}
