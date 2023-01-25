using Godot;

namespace RandomDungeons
{
    public class UserSettings
    {
        public static UserSettings Get {get; private set;} = new UserSettings();

        public bool DisplayPerformanceStats = false;
        public float MusicVolume
        {
            get => GetBusVolumeLinear("Music");
            set => SetBusVolumeLinear("Music", value);
        }

        public static void ResetToDefault()
        {
            Get = new UserSettings();
        }

        private static float GetBusVolumeLinear(string busName)
        {
            int index = AudioServer.GetBusIndex(busName);
            float volumeDb = AudioServer.GetBusVolumeDb(index);
            return GD.Db2Linear(volumeDb);
        }

        private static void SetBusVolumeLinear(string busName, float volumeLinear)
        {
            int index = AudioServer.GetBusIndex(busName);
            float volumeDb = GD.Linear2Db(volumeLinear);
            AudioServer.SetBusVolumeDb(index, volumeDb);
        }
    }
}
