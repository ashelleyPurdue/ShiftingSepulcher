using Godot;

namespace RandomDungeons
{
    public class PerformanceDisplayPanel : Control
    {
        public override void _Process(float delta)
        {
            Visible = UserSettings.Get.DisplayPerformanceStats;
        }
    }
}
