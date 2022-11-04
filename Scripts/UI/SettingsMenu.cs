using Godot;

namespace RandomDungeons
{
    public class SettingsMenu : Node
    {
        private Slider _musicVolume => GetNode<Slider>("%MusicVolumeSlider");

        public override void _Process(float delta)
        {
            _musicVolume.Value = UserSettings.Get.MusicVolume;
        }

        public void OnMusicVolumeSliderChanged(float value)
        {
            UserSettings.Get.MusicVolume = value;
        }
    }
}
