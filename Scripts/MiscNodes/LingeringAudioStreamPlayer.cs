using Godot;

namespace ShiftingSepulcher
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
        [Export] public bool Autoplay = false;

        [Export] public string Bus = "Sound";

        public override void _Ready()
        {
            if (Autoplay)
                Play();
        }

        public void Play()
        {
            var audioPlayer = new AudioStreamPlayer();
            audioPlayer.Stream = Sound;
            audioPlayer.VolumeDb = GD.Linear2Db(VolumeLinear);
            audioPlayer.Bus = Bus;

            GetTree().Root.AddChild(audioPlayer);
            audioPlayer.Play();
            audioPlayer.Connect("finished", audioPlayer, "queue_free");
        }
    }
}
