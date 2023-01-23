using Godot;

namespace RandomDungeons
{
    public class SettingsMenu : Node
    {
        private Slider _musicVolume => GetNode<Slider>("%MusicVolumeSlider");
        private CheckButton _showPerfStatsButton => GetNode<CheckButton>("%ShowPerfStatsButton");

        public override void _Process(float delta)
        {
            _musicVolume.Value = UserSettings.Get.MusicVolume;

            _showPerfStatsButton.Pressed = UserSettings.Get.DisplayPerformanceStats;
        }

        public void OnMusicVolumeSliderChanged(float value)
        {
            UserSettings.Get.MusicVolume = value;
        }

        public void OnShowPerfStatsButtonToggled(bool value)
        {
            UserSettings.Get.DisplayPerformanceStats = value;
        }
    }
}
