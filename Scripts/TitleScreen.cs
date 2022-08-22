using Godot;

namespace RandomDungeons
{
    public class TitleScreen : Control
    {
        public static int ChosenSeed;

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

        private void Play()
        {
            bool isNumber = int.TryParse(_seedTextBox.Text, out ChosenSeed);
            if (!isNumber)
                ChosenSeed = (int)_seedTextBox.Text.Hash();

            GetTree().ChangeScene("res://Maps/Main.tscn");
        }

    }
}


