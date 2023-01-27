using Godot;

namespace RandomDungeons
{
    [CustomNode("Resource", icon: "AudioStreamPlayer")]
    public class BackgroundMusicSong : Resource
    {
        [Export] public AudioStream Song;
        [Export] public float VolumeLinear = 1;
    }
}
