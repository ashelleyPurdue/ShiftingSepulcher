using Godot;

namespace RandomDungeons
{
    /// <summary>
    /// Use this to play sound effects that continue playing (or "linger") even
    /// after this node has been destroyed.
    /// </summary>
    [CustomNode(icon: "AudioStreamPlayer")]
    public class LingeringAudioStreamPlayer : Node
    {
        [Export] public AudioStream Sound;
        [Export] public float VolumeLinear = 1;

        public void Play()
        {
            var audioPlayer = new AudioStreamPlayer();
            audioPlayer.Stream = Sound;
            audioPlayer.VolumeDb = GD.Linear2Db(VolumeLinear);

            GetTree().Root.AddChild(audioPlayer);
            audioPlayer.Play();
            audioPlayer.Connect("finished", audioPlayer, "queue_free");
        }
    }
}
