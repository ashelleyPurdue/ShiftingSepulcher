using Godot;

namespace RandomDungeons
{
    /// <summary>
    /// A wrapper around <see cref="MusicService"/> that can be accessed from
    /// an animation.  It can also be used as a handler for signals.
    /// </summary>
    public class MusicServiceAccessor : Node
    {
        public float VolumeMult
        {
            get => MusicService.Instance.VolumeMult;
            set => MusicService.Instance.VolumeMult = value;
        }

        public void PlaySong(AudioStream song) => MusicService.Instance.PlaySong(song);
        public void StopSong() => MusicService.Instance.StopSong();
    }
}