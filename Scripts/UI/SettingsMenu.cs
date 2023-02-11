using Godot;

namespace ShiftingSepulcher
{
    public class SettingsMenu : Node
    {
        private Slider _masterVolume => GetNode<Slider>("%MasterVolumeSlider");
        private Slider _musicVolume => GetNode<Slider>("%MusicVolumeSlider");
        private Slider _soundVolume => GetNode<Slider>("%SoundVolumeSlider");

        private CheckButton _showPerfStatsButton => GetNode<CheckButton>("%ShowPerfStatsButton");

        public override void _Process(float delta)
        {
            _masterVolume.Value = UserSettings.Get.MasterVolume;
            _musicVolume.Value = UserSettings.Get.MusicVolume;
            _soundVolume.Value = UserSettings.Get.SoundVolume;

            _showPerfStatsButton.Pressed = UserSettings.Get.DisplayPerformanceStats;
        }

        public void OnMasterVolumeSliderChanged(float value)
        {
            UserSettings.Get.MasterVolume = value;
        }

        public void OnMusicVolumeSliderChanged(float value)
        {
            UserSettings.Get.MusicVolume = value;
        }

        public void OnSoundVolumeSliderChanged(float value)
        {
            UserSettings.Get.SoundVolume = value;
        }

        public void OnShowPerfStatsButtonToggled(bool value)
        {
            UserSettings.Get.DisplayPerformanceStats = value;
        }
    }
}
