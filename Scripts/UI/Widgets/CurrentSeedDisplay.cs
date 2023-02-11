using Godot;

namespace ShiftingSepulcher
{
    public class CurrentSeedDisplay : LineEdit
    {
        public override void _EnterTree()
        {
            Text = TitleScreen.ChosenSeed.ToString();
        }
    }
}
