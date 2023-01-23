namespace RandomDungeons
{
    public class UserSettings
    {
        public static UserSettings Get {get; private set;} = new UserSettings();

        public bool DisplayPerformanceStats = false;
        public float MusicVolume = 1;

        public static void ResetToDefault()
        {
            Get = new UserSettings();
        }
    }
}
