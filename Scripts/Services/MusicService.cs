using Godot;

namespace ShiftingSepulcher
{
    public class MusicService : Node
    {
        public static MusicService Instance {get; private set;}

        private AudioStreamPlayer _audioPlayer => GetNode<AudioStreamPlayer>("%AudioPlayer");

        public override void _Ready()
        {
            Instance = this;
        }

        public void PlaySong(BackgroundMusicSong song)
        {
            _audioPlayer.Stop();

            _audioPlayer.Stream = song.Song;
            _audioPlayer.VolumeDb = GD.Linear2Db(song.VolumeLinear);

            _audioPlayer.Play();
        }

        public void StopSong()
        {
            _audioPlayer.Stop();
        }
    }
}
