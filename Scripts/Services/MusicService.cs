using Godot;

namespace RandomDungeons
{
    public class MusicService : Node
    {
        public static MusicService Instance {get; private set;}

        private AudioStreamPlayer _audioPlayer => GetNode<AudioStreamPlayer>("%AudioPlayer");

        public override void _Ready()
        {
            Instance = this;
        }

        public void PlaySong(AudioStream song)
        {
            _audioPlayer.Stop();
            _audioPlayer.Stream = song;
            _audioPlayer.Play();
        }

        public void StopSong()
        {
            _audioPlayer.Stop();
        }
    }
}
