using Godot;

namespace RandomDungeons
{
    public class MusicService : Node
    {
        public static MusicService Instance {get; private set;}

        public float VolumeMult = 1;

        private AudioStreamPlayer _audioPlayer => GetNode<AudioStreamPlayer>("%AudioPlayer");

        public override void _Ready()
        {
            Instance = this;
        }

        public override void _Process(float delta)
        {
            _audioPlayer.VolumeDb = UserSettings.Get.MusicVolume * VolumeMult;
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
