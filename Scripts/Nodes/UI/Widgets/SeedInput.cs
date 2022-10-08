using Godot;

namespace RandomDungeons.Nodes.UI.Widgets
{
    public class SeedInput : Control
    {
        private LineEdit _seedTextBox => GetNode<LineEdit>("%SeedTextBox");

        public override void _Ready()
        {
            RandomizeSeed();
        }

        public void RandomizeSeed()
        {
            GD.Randomize();
            _seedTextBox.Text = "" + GD.Randi();
        }

        public int ParseSeedTextbox()
        {
            bool isNumber = int.TryParse(_seedTextBox.Text, out int seed);
            if (!isNumber)
                seed = (int)_seedTextBox.Text.Hash();

            return seed;
        }
    }
}
