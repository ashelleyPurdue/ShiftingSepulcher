using Godot;

namespace ShiftingSepulcher
{
    public class PerformanceDisplayPanel : Control
    {
        public override void _Process(float delta)
        {
            Visible = UserSettings.Get.DisplayPerformanceStats;
        }
    }
}
